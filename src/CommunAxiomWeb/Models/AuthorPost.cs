using System.Collections.Generic;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace CommunAxiomWeb.Models
{
    [PostType(Title = "Author post")]
    [ContentTypeRoute(Title = "Default", Route = "/authorpost")]
    public class AuthorPost : Post<AuthorPost>
    {
        public class CardRegion
        {
            [Field]
            public StringField Name { get; set; }
            [Field]
            public StringField Title { get; set; }
            [Field]
            public StringField Organisation { get; set; }
            [Field]
            public ImageField Image { get; set; }
            [Field]
            public HtmlField Body { get; set; }

        }

        [Region]
        public CardRegion Card { get; set; }
    }
}