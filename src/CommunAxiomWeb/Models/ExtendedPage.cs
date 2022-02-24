using CommunAxiomWeb.Extend;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.AttributeBuilder;
using Piranha.Models;
using System;
using System.Threading.Tasks;

namespace CommunAxiomWeb.Models
{
    
    public class ExtendedPage<T> : Piranha.AspNetCore.Models.SinglePage<T> where T : PageBase
    {
        private readonly IServiceProvider _serviceProvider;
        public ExtendedPage(IApi api, IModelLoader loader, IServiceProvider serviceProvider) : base(api, loader)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            try
            {
                Data = await _loader.GetPageAsync<T>(id, HttpContext.User, draft);

                if (Data == null)
                {
                    return NotFound();
                }

                foreach(var b in Data.Blocks)
                {
                    if(b is IExtendedBlock)
                    {
                        await (b as IExtendedBlock).Extend(HttpContext.User, this._api, this._loader, _serviceProvider, draft);
                    }
                }

                return Page();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
