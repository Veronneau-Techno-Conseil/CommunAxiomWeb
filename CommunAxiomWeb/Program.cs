using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommunAxiomWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static void BuildConfig(IConfigurationBuilder cb){
            cb.AddJsonFile("./appSettings.json")
                .AddEnvironmentVariables();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            ConfigurationBuilder cb = new ConfigurationBuilder();
            BuildConfig(cb);
            var config = cb.Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(x=> BuildConfig(x))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.ConfigureKestrel(cfg => {
                        cfg.ConfigureEndpointDefaults(opts =>
                        {
                            X509Certificate2 cert = null;
                            if (config["HostCertSource"] == "subject")
                            {
                                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                                store.Open(OpenFlags.ReadOnly);
                                cert = store.Certificates.Find(X509FindType.FindBySubjectName, config["HostCertificate"], true)[0];
                            }
                            else
                            {
                                cert = new X509Certificate2(File.ReadAllBytes(config["HostCertificate"]), config["HostPassword"]);
                            }
                            opts.UseHttps(cert);
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(config["Urls"]);
                });
        }
    }
}
