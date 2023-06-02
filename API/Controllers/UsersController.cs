using System.Net.Http.Headers;
using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _uesrRepo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository uesrRepo, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _uesrRepo = uesrRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
          
          var currentUser = await _uesrRepo.GetUserByUsernameAsync(User.GetUserName());
          userParams.CurrentUsername = currentUser.UserName;
          
          if(string.IsNullOrEmpty(userParams.Gender)){
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
          }

          var users = await _uesrRepo.GetMembersAsync(userParams);
          Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
          return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return Ok(await _uesrRepo.GetMemberAsync(username));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _uesrRepo.GetUserByUsernameAsync(User.GetUserName());

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _uesrRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
             var user = await _uesrRepo.GetUserByUsernameAsync(User.GetUserName());

             if(user == null) return NotFound();

             var result = await _photoService.AddPhotoAsync(file);

             if(result.Error != null) return BadRequest(result.Error.Message);

             var photo = new Photo
             {
               Url = result.SecureUrl.AbsoluteUri,
               PublicId = result.PublicId
             };

             if(user.Photos.Count == 0) photo.IsMain = true;

             user.Photos.Add(photo);

             if(await _uesrRepo.SaveAllAsync()) 
             {
                return CreatedAtAction(nameof(GetUser),
                    new {username = user.UserName},
                    _mapper.Map<PhotoDto>(photo));
              };

             return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _uesrRepo.GetUserByUsernameAsync(User.GetUserName());

            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x=> x.IsMain);

            if(currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _uesrRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Probem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uesrRepo.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x=> x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your main photo");

            if(photo.PublicId != null){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _uesrRepo.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting photo");
        }


    }
}