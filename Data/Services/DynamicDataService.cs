using Microsoft.EntityFrameworkCore;
using BlazorDynamicApp.Data.Models;
using BlazorDynamicApp.Data.Context;

namespace BlazorDynamicApp.Data.Services;

public class DynamicDataService(AppDbContext context, ILogger<DynamicDataService> logger) : IDynamicDataService
{
    public async Task<List<DynamicEntity>> GetAllAsync()
    {
        try
        {
            logger.LogInformation("Getting all dynamic entities");
            return await context.DynamicEntities
                .OrderBy(e => e.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all entities");
            throw;
        }
    }

    public async Task<DynamicEntity?> GetByIdAsync(int id)
    {
        return await context.DynamicEntities.FindAsync(id);
    }

    public async Task<DynamicEntity> CreateAsync(DynamicEntity entity)
    {
        try
        {
            logger.LogInformation("Creating new entity: {EntityName}", entity.Name);

            entity.CreatedDate = DateTime.UtcNow;
            context.DynamicEntities.Add(entity);
            await context.SaveChangesAsync();

            logger.LogInformation("Entity created successfully with ID: {EntityId}", entity.Id);
            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating entity: {EntityName}", entity.Name);
            throw;
        }
    }

    public async Task<DynamicEntity> UpdateAsync(DynamicEntity entity)
    {
        try
        {
            logger.LogInformation("Updating entity with ID: {EntityId}", entity.Id);

            var existingEntity = await context.DynamicEntities.FindAsync(entity.Id);
            if (existingEntity is null)
                throw new ArgumentException($"Entity with ID {entity.Id} not found");

            // Update properties
            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.Category = entity.Category;
            existingEntity.Price = entity.Price;
            existingEntity.Quantity = entity.Quantity;
            existingEntity.IsActive = entity.IsActive;
            existingEntity.ModifiedDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            logger.LogInformation("Entity updated successfully: {EntityId}", entity.Id);
            return existingEntity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating entity with ID: {EntityId}", entity.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await context.DynamicEntities.FindAsync(id);
            if (entity is null)
                return false;

            context.DynamicEntities.Remove(entity);
            await context.SaveChangesAsync();

            logger.LogInformation("Entity deleted successfully: {EntityId}", id);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting entity with ID: {EntityId}", id);
            throw;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await context.DynamicEntities
            .Select(e => e.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}