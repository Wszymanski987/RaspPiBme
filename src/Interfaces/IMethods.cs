using Microsoft.Extensions.DependencyInjection;

namespace RaspPiBme
{
    public interface IMethods
    {
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}