﻿namespace TMWalks.API;

public interface IWalkRepository
{
    Task<Walk> CreateAsync(Walk walk);
    Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sordBy = null ,bool isAcsending = true, int pageNumber = 1, int pageSize = 1000);
    Task<Walk?> GetByIdAsync(Guid id);
    Task<Walk?> UpdateAsync(Guid id, Walk walk);
    Task<Walk?> DeleteAsync(Guid id);
}
