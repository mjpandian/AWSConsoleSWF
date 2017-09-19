using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSConsoleSWF
{
    public static class WorkflowRegistration
    {

        public static void RegisterWorkflow(IAmazonSimpleWorkflow swfClient, string name, string domainName)
        {
            // Register workflow type if not already registered
            var listWorkflowRequest = new ListWorkflowTypesRequest()
            {
                Name = name,
                Domain = domainName,
                RegistrationStatus = RegistrationStatus.REGISTERED
            };
            if (swfClient.ListWorkflowTypes(listWorkflowRequest).WorkflowTypeInfos.TypeInfos.FirstOrDefault(
                                            x => x.WorkflowType.Version == "2.0") == null)
            {
                RegisterWorkflowTypeRequest request = new RegisterWorkflowTypeRequest()
                {
                    DefaultChildPolicy = ChildPolicy.TERMINATE,
                    DefaultExecutionStartToCloseTimeout = "300",
                    DefaultTaskList = new TaskList()
                    {
                        Name = "HelloWorld" // Decider need to poll for this task
                    },
                    DefaultTaskStartToCloseTimeout = "150",
                    Domain = domainName,
                    Name = name,
                    Version = "2.0"
                };

                swfClient.RegisterWorkflowType(request);

                Console.WriteLine("Setup: Registerd Workflow Name - " + request.Name);
            }
        }
    }
}
