using Piranha.Extend;
using Piranha.Extend.Fields;

namespace CommunAxiomWeb.Extend
{
    [BlockType(Name = "FormIOSegment", Category = "Content", Icon = "fas fa-edit")]
    public class FormIOSegment: Block
    {
        public TextField Title { get; set; }
        public TextField FormUrl { get; set; }

    }
}
