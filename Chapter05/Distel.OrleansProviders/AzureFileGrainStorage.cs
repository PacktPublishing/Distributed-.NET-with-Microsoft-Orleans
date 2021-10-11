using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Distel.OrleansProviders.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Distel.OrleansProviders
{
    /// <summary>
    /// Azure Files storage Provider.
    /// Persist Grain State in a Azure File share.
    /// </summary>
    public class AzureFileGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly string _serviceId;
        private readonly string _name;
        private readonly ILogger _logger;
        private readonly AzureFileStorageOptions _options;
        internal ShareClient _shareClient; 


        public AzureFileGrainStorage(string name, AzureFileStorageOptions options, IOptions<ClusterOptions> clusterOptions, ILoggerFactory loggerFactory)
        {
            this._name = name;
            var loggerName = $"{typeof(AzureFileGrainStorage).FullName}.{name}";
            this._logger = loggerFactory.CreateLogger(loggerName);
            this._options = options;
            this._serviceId = clusterOptions.Value.ServiceId;
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<AzureFileGrainStorage>(this._name), this._options.InitStage, this.Init, this.Close);
        }

        public async Task Init(CancellationToken ct)
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                var initMsg = string.Format("Init: Name={0} ServiceId={1} Share={2}", this._name, this._serviceId, this._options.Share);

                this._logger.LogInformation($"Azure File Storage Grain Storage {this._name} is initializing: {initMsg}");
                this._shareClient = new ShareClient(this._options.ConnectionString, this._options.Share.ToLower());
                await this._shareClient.CreateIfNotExistsAsync();
                stopWatch.Stop();
                this._logger.LogInformation($"Initializing provider {this._name} of type {this.GetType().Name} in stage {this._options.InitStage} took {stopWatch.ElapsedMilliseconds} Milliseconds.");
            }
            catch (Exception exc)
            {
                stopWatch.Stop();
                this._logger.LogError($"Initialization failed for provider {this._name} of type {this.GetType().Name} in stage {this._options.InitStage} in {stopWatch.ElapsedMilliseconds} Milliseconds.", exc);
                throw;
            }
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (this._shareClient == null) throw new ArgumentException("GrainState collection not initialized.");

            string id = this.GetFileName(grainReference);
            if (this._logger.IsEnabled(LogLevel.Trace))
                this._logger.Trace("Reading: Grainid={1} of GrainType={1} from storage {2}", id, grainType, this._options.Share);

            try
            {
                var directory = this._shareClient.GetDirectoryClient(grainType);
                if (await directory.ExistsAsync())
                {
                    var file = directory.GetFileClient(id);
                    if (await file.ExistsAsync())
                    {
                        ShareFileDownloadInfo download = await file.DownloadAsync();
                        using (StreamReader reader = new StreamReader(download.Content))
                        using (JsonTextReader jsonReader = new JsonTextReader(reader))
                        {
                            JsonSerializer ser = new JsonSerializer();
                            grainState.State = ser.Deserialize(jsonReader, grainState.State.GetType());
                        }

                        grainState.RecordExists = true;
                        grainState.ETag = download.Details.ETag.ToString();
                    }
                }
               
                if (grainState.State == null)
                {
                    grainState.State = Activator.CreateInstance(grainState.State.GetType());
                    grainState.RecordExists = true;
                }
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, $"Failure reading state for Grain Type {grainType} with Id {id}.");
                throw;
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (this._shareClient == null) throw new ArgumentException("GrainState collection not initialized.");

            string id = this.GetFileName(grainReference);

            if (this._logger.IsEnabled(LogLevel.Trace))
                this._logger.Trace("Writing: GrainId={0} of GrainType={1} with ETag={3} to storage {4} ", id, grainType, grainState.ETag, this._options.Share);

            var directory = this._shareClient.GetDirectoryClient(grainType);
            await directory.CreateIfNotExistsAsync();

            ShareFileClient file = directory.GetFileClient(id);
            try
            {

                var isfileExists = await file.ExistsAsync();

                if (isfileExists)
                {
                    var properties = file.GetProperties();
                    if (properties.Value.ETag.ToString() != grainState.ETag)
                    {
                        throw new InconsistentStateException("State mismatch", grainState.ETag, properties.Value.ETag.ToString());
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                using (StreamWriter writer = new StreamWriter(stream))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, grainState.State);
                    jsonWriter.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    await file.CreateAsync(stream.Length);
                    var response = await file.UploadAsync(stream);
                    grainState.ETag = response.Value.ETag.ToString();
                }

                grainState.RecordExists = true;
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, $"Failure writing state for Grain Type {grainType} with Id {id}.");
                throw;
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (this._shareClient == null) throw new ArgumentException("GrainState collection not initialized.");

            string id = this.GetFileName(grainReference);
            if (this._logger.IsEnabled(LogLevel.Trace)) this._logger.Trace(
                "Clearing: GrainType={0} Key={1} Grainid={2} ETag={3}",
                grainType, id, grainReference, grainState.ETag);


            try
            {
                var directory = this._shareClient.GetDirectoryClient(grainType);
                if (await directory.ExistsAsync())
                {
                    ShareFileClient file = directory.GetFileClient(id);
                    await file.DeleteIfExistsAsync();
                }
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, $"Failure clearing state for Grain Type {grainType} with Id {id}.");
                throw;
            }
        }

        public Task Close(CancellationToken ct)
        {
            this._shareClient = null;
            return Task.CompletedTask;
        }



        private const string KeyStringSeparator = "__";
        private string GetFileName(GrainReference grainReference) => $"{this._serviceId}{KeyStringSeparator}{grainReference.ToKeyString()}";

    }

    public static class AzureFileGrainStorageFactory
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            var options = services.GetRequiredService<IOptionsMonitor<AzureFileStorageOptions>>().Get(name);
            return ActivatorUtilities.CreateInstance<AzureFileGrainStorage>(services, options, name);
        }
    }
}
