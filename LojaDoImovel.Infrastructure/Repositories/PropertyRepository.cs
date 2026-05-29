using LojaDoImovel.Infrastructure.Data;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LojaDoImovel.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Property>> GetAllPropertiesAsync(int idEnterprise)
    {
        return await _context.Properties
            .Where(a => a.EnterpriseId == idEnterprise)
            .Include(a => a.Images)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Property?> GetPropertyByIdAsync(int idProperty, int idEnterprise)
    {
        return await _context.Properties.FirstOrDefaultAsync(a => a.Id == idProperty && a.EnterpriseId == idEnterprise);
    }

    public async Task<Property?> GetPropertyByCodeAsync(string code, int idEnterprise)
    {
        return await _context.Properties.FirstOrDefaultAsync(a => a.Code == code && a.EnterpriseId == idEnterprise);
    }

    public async Task<Property> AddPropertyAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
        await _context.SaveChangesAsync();

        return property;
    }

    public async Task<Property?> UpdatePropertyAsync(Property property)
    {
        var exists = await _context.Properties.FirstOrDefaultAsync(a => a.Id == property.Id);

        if (exists is null)
            return null;

        await _context.Properties
            .Where(a => a.Id == property.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.Title, property.Title)
                .SetProperty(a => a.Description, property.Description)
                .SetProperty(a => a.Code, property.Code)
                .SetProperty(a => a.SalePrice, property.SalePrice)
                .SetProperty(a => a.RentalPrice, property.RentalPrice)
                .SetProperty(a => a.CondoFee, property.CondoFee)
                .SetProperty(a => a.PropertyTax, property.PropertyTax)
                .SetProperty(a => a.Street, property.Street)
                .SetProperty(a => a.Number, property.Number)
                .SetProperty(a => a.Complement, property.Complement)
                .SetProperty(a => a.Neighborhood, property.Neighborhood)
                .SetProperty(a => a.City, property.City)
                .SetProperty(a => a.State, property.State)
                .SetProperty(a => a.ZipCode, property.ZipCode)
                .SetProperty(a => a.Bedrooms, property.Bedrooms)
                .SetProperty(a => a.Suites, property.Suites)
                .SetProperty(a => a.Bathrooms, property.Bathrooms)
                .SetProperty(a => a.LivingRooms, property.LivingRooms)
                .SetProperty(a => a.ParkingSpaces, property.ParkingSpaces)
                .SetProperty(a => a.PrivateArea, property.PrivateArea)
                .SetProperty(a => a.TotalArea, property.TotalArea)
                .SetProperty(a => a.PropertyType, property.PropertyType)
                .SetProperty(a => a.Purpose, property.Purpose)
                .SetProperty(a => a.IsPublished, property.IsPublished)
                .SetProperty(a => a.IsFeatured, property.IsFeatured)
                .SetProperty(a => a.VideoUrl, property.VideoUrl)
                .SetProperty(a => a.UpdatedAt, DateTime.UtcNow)
            );

        if (property.Images != null)
        {
            var existingImages = await _context.PropertyImages
                .Where(i => i.PropertyId == property.Id)
                .ToListAsync();

            var incomingPaths = property.Images.Select(i => i.ImagePath).ToHashSet();
            var toDelete = existingImages
                .Where(i => !incomingPaths.Contains(i.ImagePath))
                .ToList();

            if (toDelete.Any())
            {
                var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                foreach (var img in toDelete)
                {
                    var filePath = Path.Combine(wwwroot, img.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                _context.PropertyImages.RemoveRange(toDelete);
            }

            var existingPaths = existingImages.Select(i => i.ImagePath).ToHashSet();
            var newImages = property.Images
                .Where(i => !existingPaths.Contains(i.ImagePath))
                .Select(i => new PropertyImage
                {
                    PropertyId = property.Id,
                    ImagePath = i.ImagePath,
                    IsMain = i.IsMain,
                    DisplayOrder = i.DisplayOrder,
                })
                .ToList();

            if (newImages.Any())
                await _context.PropertyImages.AddRangeAsync(newImages);
        }

        await _context.SaveChangesAsync();
        return property;
    }

    public async Task<bool> DeletePropertyAsync(int idProperty)
    {
        var property = await _context.Properties.Include(p => p.Images).FirstOrDefaultAsync(a => a.Id == idProperty);

        if (property is null)
            return false;

        if(property.Images != null && property.Images.Any())
        {
            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            foreach (var image in property.Images)
            {
                var filePath = Path.Combine(wwwroot, image.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        await _context.Properties.Where(a => a.Id == idProperty).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();

        return true;
    }
}
