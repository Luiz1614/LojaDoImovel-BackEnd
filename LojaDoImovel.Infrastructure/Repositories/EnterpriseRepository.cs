using LojaDoImovel.Infrastructure.Data;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LojaDoImovel.Infrastructure.Repositories;

public class EnterpriseRepository : IEnterpriseRepository
{
    private readonly AppDbContext _context;

    public EnterpriseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Enterprise>> GetAllEnterprisesAsync()
    {
        return await _context.Enterprises.Where(a => a.IsActive == true).AsNoTracking().ToListAsync();
    }

    public async Task<Enterprise?> GetEnterpriseByIdAsync(int idEnterprise)
    {
        return await _context.Enterprises.FirstOrDefaultAsync(a => a.Id == idEnterprise && a.IsActive == true);
    }

    public async Task<Enterprise?> GetEnterpriseByNameAsync(string name)
    {
        return await _context.Enterprises.FirstOrDefaultAsync(a => a.Name == name && a.IsActive == true);
    }

    public async Task<Enterprise> AddEnterpriseAsync(Enterprise enterprise)
    {
        await _context.Enterprises.AddAsync(enterprise);
        await _context.SaveChangesAsync();

        return enterprise;
    }

    public async Task<Enterprise?> UpdateEnterpriseAsync(Enterprise enterprise)
    {
        var exists = await _context.Enterprises.FirstOrDefaultAsync(a => a.Id == enterprise.Id);

        if (exists == null)
            return null;

        _ = await _context.Enterprises.ExecuteUpdateAsync(setters => setters
            .SetProperty(a => a.Name, enterprise.Name)
            .SetProperty(a => a.Slug, enterprise.Slug)
            .SetProperty(a => a.Neighborhood, enterprise.Neighborhood)
            .SetProperty(a => a.City, enterprise.City)
            .SetProperty(a => a.State, enterprise.State));

        await _context.SaveChangesAsync();

        return enterprise;
    }

    public async Task<Enterprise?> UnactivateEnterpriseAsync(Enterprise enterprise)
    {
        var exists = await _context.Enterprises.FirstOrDefaultAsync(a => a.Id == enterprise.Id);

        if (exists == null)
            return null;

        _ = await _context.Enterprises.ExecuteUpdateAsync(setters => setters
            .SetProperty(a => a.IsActive, enterprise.IsActive));

        await _context.SaveChangesAsync();

        return enterprise;
    }
}
