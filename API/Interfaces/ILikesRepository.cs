using API.DTOs;
using API.Entities;
using API.helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
         Task<UserLiked> GetUserLike(int sourceUserId, int targetUserId);
         Task<AppUser> GetUserWithLikes(int userId);
         Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);
    }
}