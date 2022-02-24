/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Piranha.AspNetCore.Http;

namespace CommunAxiomWeb.Hosting
{
    public class PiranhaStartupFilter : IStartupFilter
    {

            /// <summary>
            /// Configures the application builder.
            /// </summary>
            /// <param name="next">The next filter</param>
            /// <returns>The configure action</returns>
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    builder
                        .UseSecurityMiddleware()
                        .UseStaticFiles()
                        .UseMiddleware<RoutingMiddleware>()
                        .UseMiddleware<CustomDomainSitemapMiddleware>();
                    next(builder);
                };
            }
    }
}






