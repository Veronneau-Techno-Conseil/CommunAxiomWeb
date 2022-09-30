using CommunAxiomWeb.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiomWeb.Services
{
    public class JiraSync : BackgroundService
    {
        private readonly ILogger<JiraSync> _logger = null;
        private readonly IServiceProvider _serviceProvider = null;

        public string jql = null;
        public DateTime lastFetched = DateTime.MinValue;
        public JiraSync(ILogger<JiraSync> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Refreshing JIRA Issues at: {time}", DateTimeOffset.Now);
                try
                {
                    var jiraWorkItems = _serviceProvider.GetService<JiraWorkItems>();
                    var jira = Atlassian.Jira.Jira.CreateOAuthRestClient("https://vertechcon.atlassian.net/", "vertechcon_website", "<RSAKeyValue><Modulus>2F1pJMZf9dZffnzuNJ1pm6OFNagoJsdD17FMO+R5AxPT0ztHQeqWxN/ihajEf4VCuS076/ETSXcdlxI5SMbvaOHfUpfL2a5Io56ffK/UOoHbYcjsy0qFBtOSqSrCtiSe23Mw/CmtVLOF5xOjElAiZGx6Q/MLsXD6O8h5O0w9ehU3F9gqBmMmeSSAO0hJWbjcCVlyGXC1t4lorc2WbB0WVyQdALEpZJ8gnc1zKhe3ndpRK1DqRMWc5cZ8lanIrKWdrOGTn852/8cXRiNREYHrrs42erkJ02lh4lmcOxjgrZLOGNw2J2GiOuQjQto/bOO5Q0ZRAxh3SHwS+I9GXEeAPQ==</Modulus><Exponent>AQAB</Exponent><P>5OLk7DGVuKgZxRNMBN7NmKYlCDN//8p1tSwX4BjaJpvOHT9R9tShWba85jq0kMPUfRLBIg7JP+sSm8PpVKp9b3wptr2kizT7lSdV+1OSCakicIJb5tIacqECu8NdbmmzAwpKewSIK0boHhn5Gn409tmf1mCZkt8AiIC1A2GNDH8=</P><Q>8f7MIhdAqBPBUIuRbPRoHRuoEFtRYPI1LFSOHIXgV1FZuW4NCr/ab53v3UejoXUvz/ZtGDOhGnxzKXeEJ5XsTs1ZLjZkGbDwxz1MAJqCLWkilg45o1401QkU9RBqSSY6Szd76sT2NpEpAOlAfbynXOKvMaBe3C4DTcmYCHyBRUM=</Q><DP>TWkjjXKTtZk85fdJzZfhJxCCE8z/LG638qdQB92/4jDtu9yPhxCQ5Xu1VZRjP1bMBeUS5sbjb7e8Wmwe/SCQPeVbYk+vV9l6gQ6FuPOhv5yxZiXgmSdUBJLKDuDbN52OCqgvWn3E5AYeORgFtN0cYqRlt6aCKjc7we7tyY/zmVc=</DP><DQ>q6Nv/+IELvJME6faTrAPRE4AZxGN57gC9N1IKnzeALCndfjTcUNgV/hqT8VnDraCYftDp32/D30jZU7qwfT5NW86iKd1Wi+Ap6AXTvpHyJOgP1P5l6DS0iLt5V9uM1HU2NKSppJqm4tbzNSjtErhXeU+I3G+tmZAW6TXm3VLbuc=</DQ><InverseQ>ibthhK7WjbSO1m5y8HVRusVoeG92nisg9+0j3xhyQPmqALIUQM2gubEpaaSq07E/iJoO0lfU0JolQ3QN5Q9suI1UDxdaWWxKKZ55j1+6X7sQLHQNaNXUOcyosK4O9K99xJRqjNyDjsUwoKFXiBJHAL8nnx5MecqR136bpoT8apk=</InverseQ><D>JegntbOzuER4spZ8vWcey4/YlNADSNf8gNulOZELyog8wnNORrQs+g0niLdKxfS4/ex45Rt1f5eI7aDZvHBuHXgFQlkp89dfDpuJ4bSEgEOkBfvH0M4bhFipB05gkrzTwCrsWCGNJpvVR+je1ySHAgUnXqREVjAvBy5WXxc6Zhri7lnnuEvjkQg9z23AmOZ9XGewgGLt5o36LOUVQzCU92ylfY+VPhgTANLjM4aQflwomTpuj9shSTFZbkvGL+hFeQlrnNipuLHjsAKSwgREcnBunpzdjdvS4Cxca9uCUpleTnaZ7PkVnLrPsb4ZzOAS35SPi/nEt3aXWwhraxXlmQ==</D></RSAKeyValue>", "gytju9As8cU4U1o8TnD850p49OnKyVYB", "D49oJ60YwuRauGSmFybneAv6IWEnzdZk");
                    var res = await jira.Filters.GetFilterAsync("10005");
                    if (DateTime.Now - this.lastFetched > new TimeSpan(0, 15, 0) || res.Jql != this.jql)
                    {
                        this.jql = res.Jql;

                        var opts = new Atlassian.Jira.IssueSearchOptions(this.jql);
                        opts.MaxIssuesPerRequest = 100;
                        var issues = new List<Atlassian.Jira.Issue>();
                        int issuesCount = 0;
                        int total = 0;
                        do
                        {
                            opts.StartAt = issuesCount;
                            var qres = await jira.Issues.GetIssuesFromJqlAsync(opts);
                            issuesCount += qres.Count();
                            total = qres.TotalItems;
                            issues.AddRange(qres);
                        }
                        while (total > opts.MaxIssuesPerRequest && issuesCount < total);


                        var items = new List<WorkItem>();
                        foreach (var i in issues)
                        {
                            Newtonsoft.Json.Linq.JToken startToken = null;
                            DateTime start = new DateTime(2020, 1, 1);
                            if (i.AdditionalFields.TryGetValue("customfield_10022", out startToken) && startToken != null && !string.IsNullOrWhiteSpace(startToken.ToString()))
                            {
                                try
                                {
                                    start = startToken.ToObject<DateTime>();
                                }
                                catch (Exception e)
                                {
                                    _logger.LogWarning($"Couldn't parse StartDate on issue, {e}", e);
                                }
                            }

                            Newtonsoft.Json.Linq.JToken endToken = null;
                            DateTime end = new DateTime(2024, 12, 31);
                            if (i.AdditionalFields.TryGetValue("customfield_10023", out endToken) && endToken != null && !string.IsNullOrWhiteSpace(endToken.ToString()))
                            {
                                try
                                {
                                    end = endToken.ToObject<DateTime>();
                                }
                                catch (Exception e)
                                {
                                    _logger.LogWarning($"Couldn't parse EndDate on issue, {e}", e);
                                }
                            }

                            Newtonsoft.Json.Linq.JToken prioToken = null;
                            int priocode = 99;
                            if (i.AdditionalFields.TryGetValue("customfield_10029", out prioToken) && prioToken != null && !string.IsNullOrWhiteSpace(prioToken.ToString()))
                            {
                                try
                                {
                                    priocode = prioToken.ToObject<int>();
                                }
                                catch (Exception e)
                                {
                                    _logger.LogWarning($"Couldn't parse priocode on issue, {e}", e);
                                }
                            }


                            var wi = new WorkItem()
                            {
                                Key = i.Key.Value,
                                Summary = i.Summary,
                                Status = i.Status.Name,
                                EndDate = end,
                                StartDate = start,
                                Priority = priocode
                            };

                            if (i.Components != null && i.Components.Any())
                            {
                                wi.Component = i.Components.First().Name;
                            }
                            else
                            {
                                wi.Component = "Other";
                            }

                            items.Add(wi);
                        }
                        items = items.OrderBy(X => X.StartDate).ToList();
                        jiraWorkItems.WorkItems = items;

                        items = null;
                        _logger.LogInformation("Refreshing JIRA Issues COMPLETED at: {time}", DateTimeOffset.Now);

                        await Task.Delay(60000, stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to refresh JIRA Issues");
                }
            }
        }

    }
}
