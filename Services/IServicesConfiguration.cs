using RaspPiBme.Redis.ConfigureInitializationServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RaspPiBme.Services
{
    public interface IServicesConfiguration
    {
        public void ConfigureServices(IServiceCollection serviceCollection);
    }
}