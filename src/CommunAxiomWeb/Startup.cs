using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;
using CommunAxiomWeb.Extend;
using CommunAxiomWeb.Models;
using Flagscript.PiranhaCms.Aws.S3Storage;
using CommunAxiomWeb.Services;
using Amazon.S3;
using System;
using System.Linq;
using VirtualBrowser;
namespace CommunAxiomWeb
{
    public class Startup
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The current configuration</param>
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<JiraWorkItems>(new JiraWorkItems());
            services.UseBrowser();
            // Service setup
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = true;


                options.UseCms();
                var descriptor = options.Services.FirstOrDefault(x => x.ServiceType == typeof(IStartupFilter) && x.ImplementationType?.Name == "PiranhaStartupFilter");
                if (descriptor != null)
                {
                    options.Services.Remove(descriptor);
                }
                options.Services.AddTransient<IStartupFilter, Hosting.PiranhaStartupFilter>(); 
                options.UseManager();

                if (_config.GetValue<string>("StorageMode") == "local")
                {
                    options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                }
                else
                {
                    services.AddAWSService<IAmazonS3>();
                    var aWSOptions = _config.GetAWSOptions();
                    services.AddDefaultAWSOptions(aWSOptions);
                    var cfg = new Config.PiranhaS3Config();
                    _config.Bind("PiranhaS3", cfg);
                    options.Services.AddPiranhaS3Storage(new PiranhaS3StorageOptions
                    {
                        BucketName = cfg.BucketName,
                        PublicUrlRoot = cfg.PublicUrlRoot,
                        KeyPrefix = cfg.KeyPrefix
                    }, aWSOptions);
                }

                options.UseImageSharp();
                options.UseTinyMCE();
                options.UseMemoryCache();

                options.UseEF<SQLiteDb>(db => db.UseSqlite(_config.GetConnectionString("piranha")));
                options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(_config.GetConnectionString("piranha")));


                Piranha.App.Blocks.Register<RowBlock>();
                Piranha.App.Blocks.Register<PostRow>();
                Piranha.App.Blocks.Register<AuthorLink>();
                Piranha.App.Blocks.Register<ImageSegmentBlock>();
                Piranha.App.Blocks.Register<SmartAnchorBlock>();
                Piranha.App.Blocks.Register<FormIOSegment>();
                
                Piranha.App.Modules.Manager().Scripts.Add("~/assets/js/vuecomponents.js");

                /***
                 * Here you can configure the different permissions
                 * that you want to use for securing content in the
                 * application.
                options.UseSecurity(o =>
                {
                    o.UsePermission("WebUser", "Web User");
                });
                 */
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Piranha
            App.Init(api);

            // Build content types
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build();

            // Configure Tiny MCE
            EditorConfig.FromFile("editorconfig.json");

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });

        }
    }
}
