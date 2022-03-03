using Azure.Storage.Files.Shares;
using Orleans;
using Orleans.Runtime;

namespace Distel.OrleansProviders.Options
{
    public class AzureFileStorageOptions
    {
        internal const string ORLEANS_STORAGE_SHARE = "orleansstorage";

        [Redact]
        public string ConnectionString { get; set; }

        public string Share { get; set; } = ORLEANS_STORAGE_SHARE;

        public int InitStage { get; set; } = DEFAULT_INIT_STAGE;

        public const int DEFAULT_INIT_STAGE = ServiceLifecycleStage.ApplicationServices;
    }
}