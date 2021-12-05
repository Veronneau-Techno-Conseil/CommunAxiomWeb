//using Amazon.Runtime;
//using Amazon.SecurityToken;
//using Amazon.SecurityToken.Model;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Options;
//using System.Threading;
//using System.Threading.Tasks;

//namespace CommunAxiomWeb.Services
//{
//    public class AWSKeepAlive : BackgroundService
//    {
//        private readonly ApplicationCredentials applicationCredentials = null;

//        public AWSKeepAlive(ApplicationCredentials applicationCredentials, IOptionsMonitor<AWSCreds> options)
//        {
//            this.applicationCredentials = applicationCredentials;
//            this.aWSCreds = options.CurrentValue;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            if (this.applicationCredentials.Credentials == null || this.applicationCredentials.Credentials.Expiration < System.DateTime.Now.AddMinutes(5))
//            {
//                AmazonSecurityTokenServiceClient amazonSecurityTokenServiceClient =
//                                            new AmazonSecurityTokenServiceClient(new BasicAWSCredentials(aWSCreds.accessId, aWSCreds.key));
//                var res = await amazonSecurityTokenServiceClient.GetSessionTokenAsync();
//                this.applicationCredentials.Credentials = res.Credentials;
//            }            
//            await Task.Delay(10000);
//        }
//    }

//    public class ApplicationCredentials
//    {
//        public Credentials Credentials { get;set; }
//    }
//}
