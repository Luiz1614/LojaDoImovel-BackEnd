using LojaDoImovel.Application.Services;
using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Infrastructure.Data;
using LojaDoImovel.Infrastructure.Repositories;
using LojaDoImovel.Infrastructure.Repositories.Interfaces;
using LojaDoImovel.Transform.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace LojaDoImovel.IoC;

public class DefaultModule
{
    public static void Start(IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PgSql")));
        service.RegisterMaps();

        service.AddScoped<ITokenService, TokenService>();
        service.AddScoped<IEmailService, EmailService>();
        service.AddScoped<ICepService, CepService>();

        service.AddScoped<IPropertyRepository, PropertyRepository>();
        service.AddScoped<IPropertyService, PropertyService>();

        service.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
        service.AddScoped<IEnterpriseService, EnterpriseService>();
    }
}
