using Amazon;
using Amazon.SimpleWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;

namespace AWSConsoleSWF
{
    public static class Workflow
    {
        static string domainName = "HelloSWF6";
        static IAmazonSimpleWorkflow swfClient = new AmazonSimpleWorkflowClient(RegionEndpoint.USWest2);
        public static void RunWorkflow()
        {
            string workflowName = "HelloWorld Workflow";

            // Setup
            DomainRegistration.RegisterDomain(swfClient, domainName);
            RegisterActivites(domainName);
            WorkflowRegistration.RegisterWorkflow(swfClient,workflowName,domainName);
            InitiateWorkers();
            InitiateDeciders();
           

            //Start the workflow
            Task.Run(() => StartWorkflow(workflowName));


            Console.Read();
            DeprecateDomainRequest request = new DeprecateDomainRequest()
            {
                Name = domainName
            };
            var response = swfClient.DeprecateDomain(request);
            Console.WriteLine(response.ResponseMetadata.ToString());


            Console.Read();
        }

        private static void RegisterActivites(string domainName)
        {
            ActivityRegistration.RegisterActivity(swfClient, domainName, "Activity1A", "Activity1");
            ActivityRegistration.RegisterActivity(swfClient, domainName, "Activity1B", "Activity1");
            ActivityRegistration.RegisterActivity(swfClient, domainName, "Activity2", "Activity2");

        }

        private static void InitiateWorkers()
        {

            // Launch workers to service Activity1A and Activity1B
            //  This is acheived by sharing same tasklist name (i.e.) "Activity1"
            Task.Run(() => Workers.Worker(swfClient, domainName, "Activity1"));
            Task.Run(() => Workers.Worker(swfClient, domainName, "Activity1"));

            // Launch Workers for Activity2
            Task.Run(() => Workers.Worker(swfClient, domainName, "Activity2"));
            Task.Run(() => Workers.Worker(swfClient, domainName, "Activity2"));
        }

        private static void InitiateDeciders()
        {
            // Start the Deciders, which defines the structure/flow of Workflow
            Task.Run(() => Deciders.Decider(swfClient,domainName));
        }


        private static void StartWorkflow(string name)
        {
            string workflowID = "Hello World WorkflowID - " + DateTime.Now.Ticks.ToString();
            swfClient.StartWorkflowExecution(new StartWorkflowExecutionRequest()
            {
                Input = "{\"inputparam1\":\"value1\"}", // Serialize input to a string

                WorkflowId = workflowID,
                Domain = domainName,
                WorkflowType = new WorkflowType()
                {
                    Name = name,
                    Version = "2.0"
                }
            });
            Console.WriteLine("Setup: Workflow Instance created ID=" + workflowID);
        }



    }
}
