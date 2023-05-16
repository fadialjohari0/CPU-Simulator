using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CPU
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read data from json file. (Tasks List, Number of Processors).
            string jsonFilePath = "Tasks.Json";
            string json = File.ReadAllText(jsonFilePath);
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            int numOfProcessors = jsonObject.Processors;
            List<Task> listOfTasks = jsonObject.Tasks.ToObject<List<Task>>();

            // Sort tasks by creation time.
            TaskList sortedByCreationTime = new SortByCreationTime(listOfTasks);
            sortedByCreationTime.SortTasks();

            // Initialize a list of processors.
            IProcessorInitializer processorInitializer = new ProcessorInitializer();
            List<Processor> listOfProcessors = processorInitializer.InitializeProcessors(numOfProcessors);

            // Initialize queues for high and low priority tasks.
            TasksQueue tasksQueue = new TasksQueue();

            ITasksPriorityHandler AssignTasksToQueues = new AssigningTasksToQueues();
            AssignTasksToQueues.SeparateTasksByPriority(sortedByCreationTime, tasksQueue, ref clockCycle);

            /************************** Finished Assigning Tasks to Queues *****************************/

            // Sort tasks by requested time before assigning to processors.
            TaskList sortedByRequestedTime = new SortByRequestedTime(listOfTasks);
            sortedByRequestedTime.SortTasks();

            // Assign tasks back to queues by priority.
            ITasksPriorityHandler AssignTasksToQueues2 = new TasksPriorityHandler();
            AssignTasksToQueues2.SeparateTasksByPriority(sortedByRequestedTime, tasksQueue, ref clockCycle);

            // Assign tasks to processors.
            AssigningTasksToProcessors scheduler = new AssigningTasksToProcessors();
            scheduler.AssignTasksToProcessors(sortedByRequestedTime, tasksQueue, listOfProcessors, ref clockCycle);

            // Print output data.
            PrintOutputData outputData = new PrintOutputData();
            outputData.PrintData(listOfTasks, ref clockCycle);

            OutputFile outputFile = new OutputFile();
            outputFile.WriteOutputFile(listOfTasks, ref clockCycle);
        }
        static int clockCycle = 0;
    }
}


/*
    1- Output the data into a new file and save it in the IO folder. // Done
    2- Learn about Unit Testing and apply them on the simulator.
    3- Learn about Class Diagrams and draw one for the simulator.  
*/