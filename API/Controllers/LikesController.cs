
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class LikesController : BaseApiController
    {
        private readonly ILogger<LikesController> _logger;

        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(ILogger<LikesController> logger, 
                               IUserRepository userRepository, 
                               ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId,likedUser.Id);

            if(userLike != null) return BadRequest("You already like this user");

            userLike = new UserLiked
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likeParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, 
            users.PageSize, users.TotalCount, 
            users.TotalPages));
            return Ok(users);
        }
    }
}