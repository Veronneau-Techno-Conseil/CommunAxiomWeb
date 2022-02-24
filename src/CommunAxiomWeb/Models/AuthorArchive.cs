using Piranha.AttributeBuilder;
using Piranha.Models;

namespace CommunAxiomWeb.Models
{
    [PageType(Title = "Author Archive", UseBlocks = false, IsArchive = true)]
    [PageTypeArchiveItem(typeof(AuthorArchive))]
    [ContentTypeRoute(Title = "Default", Route = "/authorarchive")]
    public class AuthorArchive: Page<AuthorArchive>
    {
        public PostArchive<AuthorPost> Archive { get; set; }
    }
}
