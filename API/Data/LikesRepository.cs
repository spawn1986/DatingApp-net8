﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLikeAsync(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PageList<MemberDto>> GetUserLikesAsync(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();

        IQueryable<MemberDto> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(x => x.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIdsAsync(likesParams.UserId);

                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PageList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
