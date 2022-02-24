using CommunAxiomWeb.Models;
using Microsoft.AspNetCore.Http;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.AttributeBuilder;

using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CommunAxiomWeb.Extend
{
    [BlockType(Name = "Author link", Category = "References", Icon = "fas fa-link", Component = "post-block")]
    public class AuthorLink: Block, IExtendedBlock
    {
        [Field]
        public PostField Body { get; set; }

        public AuthorPost AuthorPost { get; set; }

        public async Task Extend(ClaimsPrincipal user, IApi api, IModelLoader modelLoader, IServiceProvider serviceProvider, bool draft = false)
        {
            if (this.Body.Id.HasValue)
            {
                var model = await modelLoader.GetPostAsync<AuthorPost>(this.Body.Id.Value, user, draft);
                this.AuthorPost = model;
            }
        }
    }
}
