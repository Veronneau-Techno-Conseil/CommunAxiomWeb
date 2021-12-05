using Piranha;
using Piranha.AspNetCore.Models;
using Piranha.AspNetCore.Services;
using Piranha.AttributeBuilder;
using Piranha.Models;

namespace CommunAxiomWeb.Models
{
    [PageType(Title = "Project Status", UseBlocks = false)]
    [ContentTypeRoute(Title = "Default", Route = "/project/status")]
    public class StatusPage : Page<StatusPage>
    {
      
    }

}