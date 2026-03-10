using LojaDoImovel.Application.Services;
using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LojaDoImovel.IoC;

public class DefaultModule
{
    public static void Start(IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        service.AddScoped<ITokenService, TokenService>();
    }
}
