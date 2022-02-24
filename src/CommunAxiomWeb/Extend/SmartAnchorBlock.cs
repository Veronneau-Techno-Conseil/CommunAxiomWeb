using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtualBrowser;

namespace CommunAxiomWeb.Extend
{
    [BlockType(Name = "SmartAnchor", Category = "Content", Icon = "fas fa-link")]
    public class SmartAnchorBlock : Block, IExtendedBlock
    {
        

        public TextField Url { get; set; }
        public TextField Label { get; set; }
        public CheckBoxField UseOG { get; set; }

        public Metadata OGMetadata { get; set; }

        public async Task Extend(ClaimsPrincipal user, IApi api, IModelLoader modelLoader, IServiceProvider svcs, bool draft = false)
        {
            var browser = svcs.GetService<IBrowser>();
            var res = await browser.GetMetadata(this.Url.Value);
            this.OGMetadata = res;            
        }
    }
}
