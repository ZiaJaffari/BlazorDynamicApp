using BlazorDynamicApp.Data.Models;

namespace BlazorDynamicApp.Data.Services;

public interface IDynamicDataService
{
    Task<List<DynamicEntity>> GetAllAsync();
    Task<DynamicEntity?> GetByIdAsync(int id);
    Task<DynamicEntity> CreateAsync(DynamicEntity entity);
    Task<DynamicEntity> UpdateAsync(DynamicEntity entity);
    Task<bool> DeleteAsync(int id);
    Task<List<string>> GetCategoriesAsync();
}