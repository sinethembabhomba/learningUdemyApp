using API.DTOs;
using API.Entities;
using API.Extensions;
using API.helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository,
                                  IMessageRepository messageRepository,
                                  IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage (CreateMessageDto createMessage)
        {
            var username = User.GetUserName();

            if(username == createMessage.RecipientUsername.ToLower())
              return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessage.RecipientUsername);

            if(recipient == null) return NotFound();

            var message = new Message
            {
              Sender = sender,
              Recipient = recipient,
              SenderUsername = sender.UserName,
              RecipientUsername = recipient.UserName,
              Content = createMessage.Content
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SavedAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Message was not created");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessegesForUser([FromQuery] MessageParams messageParams)
        {
          messageParams.Username = User.GetUserName();

          var messages = await _messageRepository.GetMessagesForUser(messageParams);

          Response.AddPaginationHeader( new PaginationHeader(messages.CurrentPage,messages.PageSize,
                                         messages.TotalCount, messages.TotalPages));
          return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
          var currentUsername = User.GetUserName();
          return Ok(await _messageRepository.GetMessageThread(currentUsername,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
          var username = User.GetUserName();

          var message = await _messageRepository.GetMessage(id);

          if(message.SenderUsername != username && message.RecipientUsername != username)
             return Unauthorized();
          
          if(message.SenderUsername == username) message.SenderDeleted = true;
          if(message.RecipientUsername == username) message.RecipientDeleted = true;

          if(message.SenderDeleted && message.RecipientDeleted){
            _messageRepository.DeleteMessage(message);
          }

          if(await _messageRepository.SavedAllAsync()) return Ok();

          return BadRequest("Problem deleting the message");
          
        } 
   } 
}