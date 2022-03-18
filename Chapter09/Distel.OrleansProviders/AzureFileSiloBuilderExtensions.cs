using Distel.OrleansProviders.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;

namespace Distel.OrleansProviders
{
    public static class AzureFileSiloBuilderExtensions
    {
        /// <summary>
        /// Configure silo to use azure File storage as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddAzureFileGrainStorageAsDefault(this ISiloHostBuilder builder, Action<AzureFileStorageOptions> configureOptions)
        {
            return builder.AddAzureFileGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use azure File storage for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddAzureFileGrainStorage(this ISiloHostBuilder builder, string name, Action<AzureFileStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddAzureFileGrainStorage(name, ob => ob.Configure(configureOptions)));
        }


        /// <summary>
        /// Configure silo to use azure File storage as the default grain storage.
        /// </summary>
        public static ISiloBuilder AddAzureFileGrainStorageAsDefault(this ISiloBuilder builder, Action<AzureFileStorageOptions> configureOptions)
        {
            return builder.AddAzureFileGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use azure File storage for grain storage.
        /// </summary>
        public static ISiloBuilder AddAzureFileGrainStorage(this ISiloBuilder builder, string name, Action<AzureFileStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddAzureFileGrainStorage(name, ob => ob.Configure(configureOptions)));
        }

  
        ///// <summary>
        ///// Configure silo to use azure File storage for grain storage.
        ///// </summary>
        //public static ISiloBuilder AddAzureFileGrainStorage(this ISiloBuilder builder, string name, Action<OptionsBuilder<AzureFileStorageOptions>> configureOptions = null)
        //{
        //    return builder.ConfigureServices(services => services.AddAzureFileGrainStorage(name, configureOptions));
        //}

        internal static IServiceCollection AddAzureFileGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<AzureFileStorageOptions>> configureOptions = null)
        {
            configureOptions?.Invoke(services.AddOptions<AzureFileStorageOptions>(name));
            services.ConfigureNamedOptionForLogging<AzureFileStorageOptions>(name);
            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage>(name, AzureFileGrainStorageFactory.Create)
                           .AddSingletonNamedService<ILifecycleParticipant<ISiloLifecycle>>(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }
    }
}
