using Piranha.Extend.Fields;
using Piranha.Extend;

namespace CommunAxiomWeb.Extend
{
    [BlockType(Name = "ImageSegment", Category = "Content", Icon = "fas fa-paragraph")]
    public class ImageSegmentBlock : Block
    {
        [Piranha.Extend.Field(Description = "Set image to the right.")]
        public CheckBoxField SetImageRight {get;set;}
        public ImageField ImageField { get; set; }
        public SelectField<eImageSize> ImageSize { get; set; }
        public TextField Title { get; set; }
        public HtmlField Body { get; set; }

        public enum eImageSize
        {
            Large,
            Medium,
            Small,
            xlarge
        }
    }
}