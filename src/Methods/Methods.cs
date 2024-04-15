using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RaspPiBme
{
    public class Methods : IMethods
    {
        /*public Task ConfigureInitializationServices(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }+*/

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
            serviceCollection.AddTransient<ConfigureInitializationServices>();
            //serviceCollection.AddTransient<DataServices>();

        }

    }
}