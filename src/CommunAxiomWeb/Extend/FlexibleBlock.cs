using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace CommunAxiomWeb.Extend
{
    [BlockGroupType(Name = "Flexible", Category = "Content", Icon = "fas fa-arrow-right", Display = BlockDisplayMode.Vertical)]
    public class FlexibleBlock : BlockGroup, ISearchable
    {

        [Field]
        public DataSelectField<TagItem> Tag { get; set; }

        [Field]
        public TextField Classes { get; set; }

        public string Opening
        {
            get
            {
                return $"<{Tag} class=\"{Classes.Value}\">";
            }
        }
        public string Closing
        {
            get
            {
                return $"</{Tag}>";
            }
        }

        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        public string GetIndexedContent()
        {
            var content = new StringBuilder();

            foreach (var item in Items)
            {
                if (item is ISearchable searchItem)
                {
                    var value = searchItem.GetIndexedContent();

                    if (!string.IsNullOrEmpty(value))
                    {
                        content.AppendLine(value);
                    }
                }
            }
            return content.ToString();
        }

        public class TagItem
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public static Task<TagItem> GetById(string id, IApi api)
            {
                return Task.FromResult<TagItem>(new TagItem { Id = id, Name = id });
            }

            static Task<IEnumerable<DataSelectFieldItem>> GetList(IApi api)
            {
                return Task.FromResult((IEnumerable<DataSelectFieldItem>)new DataSelectFieldItem[]
                {
                    new DataSelectFieldItem(){ Id = "div", Name="div"},
                    new DataSelectFieldItem(){ Id = "span", Name="span"},
                    new DataSelectFieldItem(){ Id = "p", Name="p"},
                    new DataSelectFieldItem(){ Id = "template", Name="template"}
                });
            }
        }
    }
}