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
            TaskPriorityHandler taskPriorityHandler = new TaskPriorityHandler();
            taskPriorityHandler.SeparateTasksByPriority(listOfTasks);

            // Two Queues for High and Low priority tasks.
            Queue<Task> highPriorityTasks = taskPriorityHandler.GetHighPriorityTasks();
            Queue<Task> lowPriorityTasks = taskPriorityHandler.GetLowPriorityTasks();


            /********************************************************************************/

            Scheduler scheduler = new Scheduler();
            scheduler.AssignTasksToProcessors(listOfProcessors, highPriorityTasks, lowPriorityTasks);

            /********************************************************************************/



        }
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
        private Queue<Task> highPriorityTasks = new Queue<Task>();
        private Queue<Task> lowPriorityTasks = new Queue<Task>();

        public void SeparateTasksByPriority(List<Task> tasks)
        {
            tasks = tasks.OrderBy(task => task.RequestedTime).ToList();

            foreach (Task task in tasks)
            {
                if (task.Priority == "High")
                {
                    highPriorityTasks.Enqueue(task);
                }
                else
                {
                    lowPriorityTasks.Enqueue(task);
                }
            }
        }

        public Queue<Task> GetLowPriorityTasks() => lowPriorityTasks;
        public Queue<Task> GetHighPriorityTasks() => highPriorityTasks;

    }

    public class Scheduler
    {
        int clockCycle = 0;
        public void AssignTasksToProcessors(List<Processor> listOfProcessors, Queue<Task> highPriorityTasks, Queue<Task> lowPriorityTasks)
        {
            foreach (var processor in listOfProcessors)
            {
                if (processor.State == ProcessorState.IDLE)
                {
                    if (highPriorityTasks.Count > 0)
                    {
                        Task task = highPriorityTasks.Dequeue();
                        processor.AssignTask(task);
                        processor.State = ProcessorState.BUSY;
                        Console.WriteLine($"{task.Id} was added to processor {processor.Id} at clock cycle {clockCycle}");
                    }
                    else if (lowPriorityTasks.Count > 0)
                    {
                        Task task = lowPriorityTasks.Dequeue();
                        processor.AssignTask(task);
                        processor.State = ProcessorState.BUSY;
                        Console.WriteLine($"{task.Id} was added to processor {processor.Id} at clock cycle {clockCycle}");

                    }
                    else
                    {
                        processor.State = ProcessorState.IDLE;
                    }
                }
                clockCycle++;
            }
        }
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
