using Piranha.AspNetCore.Services;
using Piranha.Models;
using System;
using System.Collections.Generic;

namespace CommunAxiomWeb.Helper
{
    public class SitemapHelper
    {
        public static IEnumerable<SitemapItem> GetBreadcrumbs(IApplicationService webApp)
        {
            var current = webApp.PageId;

            var parents = webApp.Site.Sitemap.FindAll(x => x.HasChild(webApp.PageId));

            foreach(var p in parents)
            {
                var crumbs = UnravelCrumbs(current, p);
                foreach(var c in crumbs)
                    yield return c;
            }

            yield break;
        }
        private static IEnumerable<SitemapItem> UnravelCrumbs(Guid pageId,  SitemapItem item)
        {
            yield return item;

            foreach(var i in item.Items)
            {
                if (i.HasChild(pageId)) {
                    var c = UnravelCrumbs(pageId, i);
                    foreach(var r in c)
                        yield return r;
                }
                else if(i.Id == pageId)
                {
                    yield return i;
                    break;
                }
            }
            yield break ;
        }
    }
}
