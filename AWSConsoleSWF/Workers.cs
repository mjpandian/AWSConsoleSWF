using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSConsoleSWF
{
    public static class Workers
    {

        public static void Worker(IAmazonSimpleWorkflow swfClient, string domainName, string tasklistName)
        {
            string prefix = string.Format("Worker{0}:{1:x} ", tasklistName,
                                System.Threading.Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                Console.WriteLine(prefix + ": Polling for activity task ...");
                PollForActivityTaskRequest pollForActivityTaskRequest =
                    new PollForActivityTaskRequest()
                    {
                        Domain = domainName,
                        TaskList = new TaskList()
                        {
                            // Poll only the tasks assigned to me
                            Name = tasklistName
                        }
                    };
                PollForActivityTaskResponse pollForActivityTaskResponse =
                                swfClient.PollForActivityTask(pollForActivityTaskRequest);

                RespondActivityTaskCompletedRequest respondActivityTaskCompletedRequest =
                            new RespondActivityTaskCompletedRequest()
                            {
                                Result = "{\"activityResult1\":\"Result Value1\"}",
                                TaskToken = pollForActivityTaskResponse.ActivityTask.TaskToken
                            };
                if (pollForActivityTaskResponse.ActivityTask.ActivityId == null)
                {
                    Console.WriteLine(prefix + ": NULL");
                }
                else
                {
                    RespondActivityTaskCompletedResponse respondActivityTaskCompletedResponse =
                        swfClient.RespondActivityTaskCompleted(respondActivityTaskCompletedRequest);
                    Console.WriteLine(prefix + ": Activity task completed. ActivityId - " +
                        pollForActivityTaskResponse.ActivityTask.ActivityId);
                }
            }
        }
    }
}
