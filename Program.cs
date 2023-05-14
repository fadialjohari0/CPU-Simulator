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
            // Reads JSON file and creates a list (to add the tasks to it through listsOfTasks).
            IFileReader tasksLoader = new TasksLoader();
            // This is a List that holds the JSON Tasks, using DeserializeObject as a for loop.
            List<Task> listOfTasks = tasksLoader.ReadTasksFromJson("Tasks.Json");

            /********************************************************************************/

            // Static function to get the input number of processors from the user.
            IUserInputHandler userInputHandler = new UserInputHandler();
            int numOfProcessors = userInputHandler.GetNumOfProcessorsFromUser();

            /********************************************************************************/

            // A List of processors depending on the `numOfProcessors` by the user.
            IProcessorInitializer processorInitializer = new ProcessorInitializer();
            List<Processor> listOfProcessors = processorInitializer.InitializeProcessors(numOfProcessors);

            /********************************************************************************/

            // Sending the `listOfTasks` to `taskPriorityHandler` to separate the tasks by priority
            // TaskPriorityHandler taskPriorityHandler = new TaskPriorityHandler();
            // taskPriorityHandler.SeparateTasksByPriority(listOfTasks);

            // Two Queues for High and Low priority tasks.
            // Queue<Task> highPriorityTasks = taskPriorityHandler.GetHighPriorityTasks();
            // Queue<Task> lowPriorityTasks = taskPriorityHandler.GetLowPriorityTasks();


            /********************************************************************************/




            Queue<Task> highPriorityTask = new Queue<Task>();
            Queue<Task> lowPriorityTask = new Queue<Task>();

            TaskPriorityHandler taskPriorityHandler = new TaskPriorityHandler();
            taskPriorityHandler.SeparateTasksByPriority(listOfTasks, highPriorityTask, lowPriorityTask, ref clockCycle);

            /********************************************************************************/

            Scheduler scheduler = new Scheduler();

            foreach (var i in highPriorityTask)
            {
                Console.WriteLine($"High priority task id: {i.Id}");
            }

            foreach (var x in lowPriorityTask)
            {
                Console.WriteLine($"Low priority task id: {x.Id}");
            }
        }

        static int clockCycle = 0;
    }


    interface IFileReader
    {
        List<Task> ReadTasksFromJson(string filePath);
    }

    public class TasksLoader : IFileReader
    {
        public List<Task> ReadTasksFromJson(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Task>>(fileContent);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            catch (JsonException)
            {
                Console.WriteLine($"Invalid JSON format in file: {filePath}");
            }

            return new List<Task>();
        }
    }

    interface IProcessorInitializer
    {
        List<Processor> InitializeProcessors(int numOfProcessors);
    }

    public class ProcessorInitializer : IProcessorInitializer
    {
        List<Processor> processors = new List<Processor>();

        public List<Processor> InitializeProcessors(int numOfProcessors)
        {
            for (int i = 0; i < numOfProcessors; i++)
            {
                Processor processor = new Processor()
                {
                    Id = $"P{i + 1}",
                    State = ProcessorState.IDLE
                };

                processors.Add(processor);
            }

            return processors;
        }
    }

    public class TaskPriorityHandler
    {
        public void SeparateTasksByPriority(List<Task> tasks, Queue<Task> highPriorityTasks, Queue<Task> lowPriorityTasks, ref int clockCycle)
        {
            // tasks = tasks.OrderBy(task => task.CreationTime).ToList();

            while (!(tasks.Count == (highPriorityTasks.Count + lowPriorityTasks.Count)))
            {
                foreach (Task task in tasks)
                {
                    if (task.CreationTime == clockCycle)
                    {
                        if (task.Priority == "High")
                        {
                            highPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                            Console.WriteLine($"Task {task.Id} is added to the Queue at clock cycle: {clockCycle}.");
                        }
                        else if (task.Priority == "Low")
                        {
                            lowPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                            Console.WriteLine($"Task {task.Id} is added to the Queue at clock cycle: {clockCycle}.");

                        }
                    }
                }
                clockCycle++;
            }
        }
    }



    public class Scheduler
    {

    }

    public enum TaskState
    {
        WAITING,
        EXECUTING,
        COMPLETED
    }

    public enum ProcessorState
    {
        BUSY,
        IDLE
    }
}
