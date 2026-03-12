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

    public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
    {
        return await _context.Properties.AsNoTracking().ToListAsync();
    }

    public async Task<Property?> GetPropertyByIdAsync(int id)
    {
        return await _context.Properties.FindAsync(id);
    }

    public async Task<Property?> GetPropertyByCodeAsync(string code)
    {
        return await _context.Properties.FirstOrDefaultAsync(a => a.Code == code);
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
        {
            return null;
        }

        _ = await _context.Properties.ExecuteUpdateAsync(setters => setters
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
            .SetProperty(a => a.UpdatedAt, DateTime.UtcNow)
            .SetProperty(a => a.Images, property.Images)
            );

        await _context.SaveChangesAsync();

        return property;
    }

    public async Task<bool> DeletePropertyAsync(int idProperty)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(a => a.Id == idProperty);

        if (property is null)
            return false;

        await _context.Properties.Where(a => a.Id == idProperty).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();

        return true;
    }
}
