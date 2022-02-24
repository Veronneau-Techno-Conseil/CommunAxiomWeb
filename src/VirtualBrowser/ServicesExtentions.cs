using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualBrowser
{
    public static class ServicesExtentions
    {
        public static IServiceCollection UseBrowser(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IBrowser, Browser>();
            return services;
        }
    }
}
