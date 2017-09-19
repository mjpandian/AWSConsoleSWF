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
    class Program
    {

        static void Main(string[] args)
        {

            Workflow.RunWorkflow();
        }

    }
}
