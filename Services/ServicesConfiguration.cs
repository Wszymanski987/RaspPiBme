using System;
using RaspPiBme.Redis.ConfigureInitializationServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RaspPiBme.Services
{
    public class ServicesConfiguration
    {
        public IConfigurationRoot _configuration;

        public ServicesConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfigurationRoot>(_configuration);
            serviceCollection.AddTransient<ConfigureInitializationTimeSeries>();
        }
    }
}

