using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSConsoleSWF
{
    public static class DomainRegistration
    {

        public static void RegisterDomain(IAmazonSimpleWorkflow swfClient, string domainName)
        {
            // Register if the domain is not already registered.
            var listDomainRequest = new ListDomainsRequest()
            {
                RegistrationStatus = RegistrationStatus.REGISTERED
            };

            if (swfClient.ListDomains(listDomainRequest).DomainInfos.Infos.FirstOrDefault(
                                                    x => x.Name == domainName) == null)
            {
                RegisterDomainRequest request = new RegisterDomainRequest()
                {
                    Name = domainName,
                    Description = "Hello World Demo",
                    WorkflowExecutionRetentionPeriodInDays = "1"
                };

                Console.WriteLine("Setup: Created Domain - " + domainName);
                swfClient.RegisterDomain(request);
            }
        }
    }
}
