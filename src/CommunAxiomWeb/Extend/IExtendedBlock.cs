using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Extend;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CommunAxiomWeb.Extend
{
    public interface IExtendedBlock
    {
        Task Extend(ClaimsPrincipal user, IApi api, IModelLoader modelLoader, IServiceProvider svcs, bool draft = false);
    }
}
