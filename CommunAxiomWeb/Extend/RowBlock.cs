using System.Text;
using Piranha.Extend;
using Piranha.Models;

namespace CommunAxiomWeb.Extend{
    [BlockGroupType(Name = "Row", Category = "Content", Icon = "fas fa-arrow-right", Display = BlockDisplayMode.Horizontal)]
    public class RowBlock : BlockGroup, ISearchable
    {
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
    }
}