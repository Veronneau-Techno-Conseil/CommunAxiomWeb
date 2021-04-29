using Piranha.Extend.Fields;
using Piranha.Extend;

namespace CommunAxiomWeb.Extend
{
    [BlockType(Name = "ImageSegment", Category = "Content", Icon = "fas fa-paragraph")]
    public class ImageSegmentBlock : Block
    {
        [Piranha.Extend.FieldDescription("Set image to the right.")]
        public CheckBoxField SetImageRight {get;set;}
        public ImageField ImageField { get; set; }
        public TextField Title { get; set; }
        public HtmlField Body { get; set; }
    }
}