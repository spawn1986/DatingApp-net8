using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLikeAsync(int sourceUserId, int targetUserId);
    Task<PageList<MemberDto>> GetUserLikesAsync(LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int currentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
}
