using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CommunAxiomWeb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            cb.AddJsonFile("./appsettings.json")
                .AddEnvironmentVariables();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            ConfigurationBuilder cb = new ConfigurationBuilder();
            BuildConfig(cb);
            var config = cb.Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hc, svcs) =>
                {
                    svcs.AddHostedService<JiraSync>();
                })
                .ConfigureAppConfiguration(x=> BuildConfig(x))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    if (config.GetValue<bool>("UseSSL"))
                    {
                        webBuilder.ConfigureKestrel(cfg =>
                        {
                            cfg.ConfigureEndpointDefaults(opts =>
                            {
                                var certPem = File.ReadAllText("cert.pem");
                                var eccPem = File.ReadAllText("key.pem");

                                var cert = X509Certificate2.CreateFromPem(certPem, eccPem);
                                cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(
                                    cert.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pkcs12));
                                opts.UseHttps(cert);
                            });
                        });
                    }
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("https://*:8443");
                });
        }
    }
}
