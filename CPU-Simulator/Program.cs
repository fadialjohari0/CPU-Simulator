using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CPU
{
    class Program
    {
        static void Main(string[] args)
        {
            // Reads JSON file and creates a list.
            IFileReader tasksLoader = new TasksLoader();
            List<Task> listOfTasks = tasksLoader.ReadTasksFromJson("Tasks.Json");

            // Sort tasks by creation time.
            TaskList sortedByCreationTime = new SortByCreationTime(listOfTasks);
            sortedByCreationTime.SortTasks();

            // Get the input number of processors from the user.
            IUserInputHandler userInputHandler = new UserInputHandler();
            int numOfProcessors = userInputHandler.GetNumOfProcessorsFromUser();

            // Initialize a list of processors.
            IProcessorInitializer processorInitializer = new ProcessorInitializer();
            List<Processor> listOfProcessors = processorInitializer.InitializeProcessors(numOfProcessors);

            // Initialize queues for high and low priority tasks.
            TasksQueue tasksQueue = new TasksQueue();

            ITasksPriorityHandler AssignTasksToQueues = new AssigningTasksToQueues();
            AssignTasksToQueues.SeparateTasksByPriority(sortedByCreationTime, tasksQueue, ref clockCycle);

            /************************** Finished Assigning Tasks to Queues *****************************/

            // Sort tasks by requested time before assigning to processors.
            TaskList sortedByRequestedTime = new SortByRequestedTime(sortedByCreationTime.Tasks);
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
        }
        static int clockCycle = 0;
    }
}


/*
    1- Output the data into a new file and save it in the IO folder.
    2- Learn about Unit Testing and apply them on the simulator.
    3- Learn about Class Diagrams and draw one for the simulator.  
*/