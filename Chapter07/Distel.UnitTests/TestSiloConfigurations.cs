using Distel.OrleansProviders;
using Orleans.Hosting;
using Orleans.TestingHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distel.UnitTests
{
    public class TestSiloConfigurations : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.AddMemoryGrainStorageAsDefault();
            siloBuilder.UseInMemoryReminderService();
            siloBuilder.AddAzureFileGrainStorage("FileShare", opt =>
            {
                opt.Share = "distelstore";
                opt.ConnectionString = "<<Update connection string>>";
            });


        }
    }
}
