using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RaspPiBme.Services.Configuration
{
    public interface IServicesConfiguration
    {
        public void ConfigureServices(IServiceCollection serviceCollection);
    }
}