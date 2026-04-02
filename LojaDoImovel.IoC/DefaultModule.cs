using LojaDoImovel.Application.Services;
using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Infrastructure.Data;
using LojaDoImovel.Infrastructure.Repositories;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using LojaDoImovel.Transform.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LojaDoImovel.IoC;

public class DefaultModule
{
    public static void Start(IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PgSql")));
        service.RegisterMaps();

        service.AddScoped<ITokenService, TokenService>();

        service.AddTransient<IPropertyRepository, PropertyRepository>();
        service.AddTransient<IPropertyService, PropertyService>();

        service.AddTransient<IEnterpriseRepository, EnterpriseRepository>();
        service.AddTransient<IEnterpriseService, EnterpriseService>();
    }
}
