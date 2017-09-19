using Amazon;
using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSConsoleSWF
{
    public static class ActivityRegistration
    {

        public static void RegisterActivity(IAmazonSimpleWorkflow swfClient, string domainName, string name, string tasklistName)
        {
            // Register activities if it is not already registered
            var listActivityRequest = new ListActivityTypesRequest()
            {
                Domain = domainName,
                Name = name,
                RegistrationStatus = RegistrationStatus.REGISTERED
            };

            if (swfClient.ListActivityTypes(listActivityRequest).ActivityTypeInfos.TypeInfos.FirstOrDefault(
                                        x => x.ActivityType.Version == "2.0") == null)
            {
                RegisterActivityTypeRequest request = new RegisterActivityTypeRequest()
                {
                    Name = name,
                    Domain = domainName,
                    Description = "Hello World Activities",
                    Version = "2.0",
                    DefaultTaskList = new TaskList() { Name = tasklistName },//Worker poll based on this
                    DefaultTaskScheduleToCloseTimeout = "300",
                    DefaultTaskScheduleToStartTimeout = "150",
                    DefaultTaskStartToCloseTimeout = "450",
                    DefaultTaskHeartbeatTimeout = "NONE",
                };
                swfClient.RegisterActivityType(request);
                Console.WriteLine("Setup: Created Activity Name - " + request.Name);
            }
        }
    }
}
