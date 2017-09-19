using Amazon.SimpleWorkflow;
using Amazon.SimpleWorkflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSConsoleSWF
{
    public static class Deciders
    {
        // Simple logic
        //  Creates four activities at the begining
        //  Waits for them to complete and completes the workflow
        public static void Decider(IAmazonSimpleWorkflow swfClient,string domainName)
        {
            int activityCount = 0; // This refers to total number of activities per workflow
            while (true)
            {
                Console.WriteLine("Decider: Polling for decision task ...");
                PollForDecisionTaskRequest request = new PollForDecisionTaskRequest()
                {
                    Domain = domainName,
                    TaskList = new TaskList() { Name = "HelloWorld" }
                };

                PollForDecisionTaskResponse response = swfClient.PollForDecisionTask(request);
                if (response.DecisionTask.TaskToken == null)
                {
                    Console.WriteLine("Decider: NULL");
                    continue;
                }

                int completedActivityTaskCount = 0, totalActivityTaskCount = 0;
                foreach (HistoryEvent e in response.DecisionTask.Events)
                {
                    Console.WriteLine("Decider: EventType - " + e.EventType +
                        ", EventId - " + e.EventId);
                    if (e.EventType == "ActivityTaskCompleted")
                        completedActivityTaskCount++;
                    if (e.EventType.Value.StartsWith("Activity"))
                        totalActivityTaskCount++;
                }
                Console.WriteLine(".... completedCount=" + completedActivityTaskCount);

                List<Decision> decisions = new List<Decision>();
                if (totalActivityTaskCount == 0) // Create this only at the begining
                {
                    ScheduleActivity("Activity1A", decisions);
                    ScheduleActivity("Activity1B", decisions);
                    ScheduleActivity("Activity2", decisions);
                    ScheduleActivity("Activity2", decisions);
                    activityCount = 4;
                }
                else if (completedActivityTaskCount == activityCount)
                {
                    Decision decision = new Decision()
                    {
                        DecisionType = DecisionType.CompleteWorkflowExecution,
                        CompleteWorkflowExecutionDecisionAttributes =
                        new CompleteWorkflowExecutionDecisionAttributes
                        {
                            Result = "{\"Result\":\"WF Complete!\"}"
                        }
                    };
                    decisions.Add(decision);

                    Console.WriteLine("Decider: WORKFLOW COMPLETE!!!!!!!!!!!!!!!!!!!!!!");
                }
                RespondDecisionTaskCompletedRequest respondDecisionTaskCompletedRequest =
                    new RespondDecisionTaskCompletedRequest()
                    {
                        Decisions = decisions,
                        TaskToken = response.DecisionTask.TaskToken
                    };
                swfClient.RespondDecisionTaskCompleted(respondDecisionTaskCompletedRequest);
            }
        }

        private static void ScheduleActivity(string name, List<Decision> decisions)
        {
            Decision decision = new Decision()
            {
                DecisionType = DecisionType.ScheduleActivityTask,
                ScheduleActivityTaskDecisionAttributes =  // Uses DefaultTaskList
                new ScheduleActivityTaskDecisionAttributes()
                {
                    ActivityType = new ActivityType()
                    {
                        Name = name,
                        Version = "2.0"
                    },
                    ActivityId = name + "-" + System.Guid.NewGuid().ToString(),
                    Input = "{\"activityInput1\":\"value1\"}"
                }
            };
            Console.WriteLine("Decider: ActivityId=" +
                        decision.ScheduleActivityTaskDecisionAttributes.ActivityId);
            decisions.Add(decision);
        }
    }
}
