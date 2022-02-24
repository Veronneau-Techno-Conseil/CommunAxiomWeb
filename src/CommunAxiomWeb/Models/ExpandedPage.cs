using Piranha.AttributeBuilder;
using Piranha.Models;

namespace CommunAxiomWeb.Models
{
    [PageType(Title = "Expanded page")]
    [ContentTypeRoute(Title = "Default", Route = "/ExtendedPage")]

    public class ExpandedPage  : Page<ExpandedPage>
    {
        
    }
}