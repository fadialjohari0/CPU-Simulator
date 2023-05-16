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
            IProcessorInitializer processorInitializer = new ProcessorInitializer();
            ITasksPriorityHandler tasksPriorityHandler = new TasksPriorityHandler();
            Scheduler scheduler = new Scheduler();
            OutputFile outputFile = new OutputFile();

            Simulator simulator = new Simulator(processorInitializer, tasksPriorityHandler, scheduler, outputFile);
            simulator.Start();
        }
    }

    public class Simulator
    {
        private int clockCycle = 0;
        private IProcessorInitializer _processorInitializer;
        private ITasksPriorityHandler _tasksPriorityHandler;
        private Scheduler _scheduler;
        private OutputFile _outputFile;

        public Simulator(
            IProcessorInitializer processorInitializer,
            ITasksPriorityHandler tasksPriorityHandler,
            Scheduler scheduler,
            OutputFile outputFile)
        {
            _processorInitializer = processorInitializer;
            _tasksPriorityHandler = tasksPriorityHandler;
            _scheduler = scheduler;
            _outputFile = outputFile;
        }

        public void Start()
        {
            var (numOfProcessors, listOfTasks) = ReadData();
            var sortedByCreationTime = SortTasksByCreationTime(listOfTasks);
            var listOfProcessors = _processorInitializer.InitializeProcessors(numOfProcessors);
            var tasksQueue = AssignTasksToQueues(sortedByCreationTime);
            var sortedByRequestedTime = SortTasksByRequestedTime(listOfTasks);

            AssignTasksToQueues2(sortedByRequestedTime, tasksQueue);
            _scheduler.AssignTasksToProcessors(sortedByRequestedTime, tasksQueue, listOfProcessors, ref clockCycle);
            _outputFile.WriteOutputFile(listOfTasks, ref clockCycle);
        }
        private (int, List<Task>) ReadData()
        {
            string jsonFilePath = "Tasks.Json";
            string json = File.ReadAllText(jsonFilePath);
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            int numOfProcessors = jsonObject.Processors;
            List<Task> listOfTasks = jsonObject.Tasks.ToObject<List<Task>>();
            return (numOfProcessors, listOfTasks);
        }

        private TaskList SortTasksByCreationTime(List<Task> listOfTasks)
        {
            TaskList sortedByCreationTime = new SortByCreationTime(listOfTasks);
            sortedByCreationTime.SortTasks();
            return sortedByCreationTime;
        }

        private List<Processor> InitializeProcessors(int numOfProcessors)
        {
            IProcessorInitializer processorInitializer = new ProcessorInitializer();
            return processorInitializer.InitializeProcessors(numOfProcessors);
        }

        private TasksQueue AssignTasksToQueues(TaskList sortedByCreationTime)
        {
            TasksQueue tasksQueue = new TasksQueue();
            ITasksPriorityHandler AssignTasksToQueues = new AssigningTasksToQueues();
            AssignTasksToQueues.SeparateTasksByPriority(sortedByCreationTime, tasksQueue, ref clockCycle);
            return tasksQueue;
        }

        private TaskList SortTasksByRequestedTime(List<Task> listOfTasks)
        {
            TaskList sortedByRequestedTime = new SortByRequestedTime(listOfTasks);
            sortedByRequestedTime.SortTasks();
            return sortedByRequestedTime;
        }

        private void AssignTasksToQueues2(TaskList sortedByRequestedTime, TasksQueue tasksQueue)
        {
            ITasksPriorityHandler AssignTasksToQueues2 = new TasksPriorityHandler();
            AssignTasksToQueues2.SeparateTasksByPriority(sortedByRequestedTime, tasksQueue, ref clockCycle);
        }

        private void AssignTasksToProcessors(TaskList sortedByRequestedTime, TasksQueue tasksQueue, List<Processor> listOfProcessors)
        {
            Scheduler scheduler = new Scheduler();
            scheduler.AssignTasksToProcessors(sortedByRequestedTime, tasksQueue, listOfProcessors, ref clockCycle);
        }

        private void WriteOutputFile(List<Task> listOfTasks)
        {
            OutputFile outputFile = new OutputFile();
            outputFile.WriteOutputFile(listOfTasks, ref clockCycle);
        }
    }

    public class Task
    {
        public string? Id { get; set; }
        public int CreationTime { get; set; }
        public int RequestedTime { get; set; }
        public int CompletionTime { get; set; }
        public string? Priority { get; set; }
        public TaskState State { get; set; }
    }

    public class Processor
    {
        public string? Id { get; set; }
        public ProcessorState State { get; set; }
        public Task? CurrentTask { get; set; }
        public void AssignTask(Task task)
        {
            CurrentTask = task;
            State = ProcessorState.BUSY;
        }
    }

    public class TasksQueue
    {
        public Queue<Task> HighPriorityTasks { get; private set; }
        public Queue<Task> LowPriorityTasks { get; private set; }
        public TasksQueue()
        {
            HighPriorityTasks = new Queue<Task>();
            LowPriorityTasks = new Queue<Task>();
        }
    }

    public interface IProcessorInitializer
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

    public abstract class TaskList
    {
        public List<Task> Tasks { get; protected set; }
        public TaskList(List<Task> tasks)
        {
            Tasks = tasks;
        }
        public abstract void SortTasks();
    }

    public class SortByCreationTime : TaskList
    {
        public SortByCreationTime(List<Task> tasks) : base(tasks) { }
        public override void SortTasks()
        {
            Tasks = Tasks.OrderBy(task => task.CreationTime).ToList();
        }
    }

    public class SortByRequestedTime : TaskList
    {
        public SortByRequestedTime(List<Task> tasks) : base(tasks) { }
        public override void SortTasks()
        {
            Tasks = Tasks.OrderBy(task => task.RequestedTime).ToList();
        }
    }

    public interface ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle);
    }

    public class AssigningTasksToQueues : ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle)
        {
            while (taskList.Tasks.Count != (tasksQueue.HighPriorityTasks.Count + tasksQueue.LowPriorityTasks.Count))
            {
                foreach (Task task in taskList.Tasks)
                {
                    if (task.CreationTime == clockCycle)
                    {
                        if (task.Priority == "High")
                        {
                            tasksQueue.HighPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                        }
                        else if (task.Priority == "Low")
                        {
                            tasksQueue.LowPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                        }
                    }
                }
                clockCycle++;
            }
        }
    }

    public class TasksPriorityHandler : ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle)
        {
            tasksQueue.HighPriorityTasks.Clear();
            tasksQueue.LowPriorityTasks.Clear();
            foreach (Task task in taskList.Tasks)
            {
                if (task.Priority == "High")
                {
                    tasksQueue.HighPriorityTasks.Enqueue(task);
                }
                else
                {
                    tasksQueue.LowPriorityTasks.Enqueue(task);
                }
            }
        }
    }
    public class Scheduler
    {
        public void AssignTasksToProcessors(TaskList taskList, TasksQueue tasksQueue, List<Processor> processors, ref int clockCycle)
        {
            while (tasksQueue.HighPriorityTasks.Count != 0 || tasksQueue.LowPriorityTasks.Count != 0 || processors.Any(processor => processor.State == ProcessorState.BUSY))
            {
                foreach (Processor processor in processors)
                {
                    if (processor.State == ProcessorState.BUSY)
                    {
                        processor.CurrentTask!.RequestedTime--;
                        if (processor.CurrentTask.RequestedTime == 0)
                        {
                            processor.CurrentTask.State = TaskState.COMPLETED;
                            processor.State = ProcessorState.IDLE;
                            processor.CurrentTask.CompletionTime = clockCycle;
                            processor.CurrentTask = null;
                        }
                    }
                    else if (processor.State == ProcessorState.IDLE)
                    {
                        if (tasksQueue.HighPriorityTasks.Count > 0)
                        {
                            Task task = tasksQueue.HighPriorityTasks.Dequeue();
                            processor.CurrentTask = task;
                            processor.State = ProcessorState.BUSY;
                            task.State = TaskState.EXECUTING;
                        }
                        else if (tasksQueue.LowPriorityTasks.Count > 0)
                        {
                            Task task = tasksQueue.LowPriorityTasks.Dequeue();
                            processor.CurrentTask = task;
                            processor.State = ProcessorState.BUSY;
                            task.State = TaskState.EXECUTING;
                        }
                    }
                }
                clockCycle++;
            }
        }
    }

    public class OutputFile
    {
        public void WriteOutputFile(List<Task> tasks, ref int clockCycle)
        {
            string filePath = Path.Combine("CPU-Simulator", "IO", "OutputData.txt");
            using (StreamWriter writetext = new StreamWriter(filePath))
            {
                writetext.WriteLine("\n---------------------------OUTPUT DATA---------------------------\n");
                writetext.WriteLine("Task ID | Creation Time | Completion Time | Priority | State");
                writetext.WriteLine("--------|---------------|-----------------|----------|-----------");
                foreach (Task task in tasks)
                {
                    writetext.WriteLine($"{task.Id,-7} | {task.CreationTime,-13} | {task.CompletionTime,-15} | {task.Priority,-8} | {task.State}");
                }
                writetext.WriteLine($"\nTotal Clock Cycles: {clockCycle}");
            }
        }
    }

    public enum ProcessorState
    {
        BUSY,
        IDLE
    }

    public enum TaskState
    {
        WAITING,
        EXECUTING,
        COMPLETED
    }
}