using CommunAxiomWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Piranha;
using Piranha.AspNetCore.Models;
using System.Security.Claims;
using Piranha.Models;

namespace CommunAxiomWeb.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IApi _api;
        private readonly IServiceProvider _serviceProvider = null;

        public ProjectController(IApi api, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _api = api;
        }

        [HttpGet]
        public IActionResult Epics()
        {
            var jwi = _serviceProvider.GetService<JiraWorkItems>();

            var components = jwi.WorkItems.GroupBy(X=>X.Component);
            var prio = components.GroupBy(X=>X.Min(y=>y.Priority)).OrderBy(X=>X.Key);

            var gpLst = new List<TLGroup>();

            foreach(var prioGp in prio){
                var ordered = prioGp.OrderBy(X=>X.Min(X=>X.StartDate));
                foreach(var gp in ordered){
                    TLGroup tgp = new TLGroup();
                    tgp.group = gp.Key;
                    var lst = new List<TLLabel>();
                    foreach(var item in gp){
                        lst.Add(new TLLabel{
                            label = item.Summary,
                            data = new TLSegments[] {
                                new TLSegments{
                                    val = item.Status,
                                    timeRange = new DateTime[]{ item.StartDate, item.EndDate }
                                }
                            }
                        });
                    }
                    tgp.data = lst.ToArray();
                    gpLst.Add(tgp);
                }
            }

            return Ok(gpLst);
        }

        //public async Task<IActionResult> Status(Guid id)
        //{
        //    var model = await _api.Pages.GetByIdAsync<StatusPage>(id);
            
        //    return View(model);
        //}
    }
}
