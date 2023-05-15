﻿using System;
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

            AssignTasksToQueues AssignTasksToQueues = new AssignTasksToQueues();
            AssignTasksToQueues.SeparateTasksByPriority(sortedByCreationTime, tasksQueue, ref clockCycle);

            /************************** Finished Assigning Tasks to Queues *****************************/

            // Sort tasks by requested time before assigning to processors.
            TaskList sortedByRequestedTime = new SortByRequestedTime(sortedByCreationTime.Tasks);
            sortedByRequestedTime.SortTasks();

            AssignTasksToProcessors scheduler = new AssignTasksToProcessors();
            scheduler.SeparateTasksByPriority(sortedByRequestedTime, tasksQueue, ref clockCycle);

            Test test = new Test();
            test.TasksToProcessors(sortedByRequestedTime, tasksQueue, listOfProcessors, ref clockCycle);



            OutputData outputData = new OutputData();
            outputData.PrintOutputData(listOfTasks, ref clockCycle);
        }

        static int clockCycle = 0;

    }

    /**********************************************************************/

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

    /**********************************************************************/

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

    /**********************************************************************/

    interface ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle);
    }

    public class AssignTasksToQueues : ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle)
        {
            while (!(taskList.Tasks.Count == (tasksQueue.HighPriorityTasks.Count + tasksQueue.LowPriorityTasks.Count)))
            {
                foreach (Task task in taskList.Tasks)
                {
                    if (task.CreationTime == clockCycle)
                    {
                        if (task.Priority == "High")
                        {
                            tasksQueue.HighPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                            Console.WriteLine($"Task {task.Id} is added to the Queue at clock cycle: {clockCycle}.");
                            Thread.Sleep(200);

                        }
                        else if (task.Priority == "Low")
                        {
                            tasksQueue.LowPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                            Console.WriteLine($"Task {task.Id} is added to the Queue at clock cycle: {clockCycle}.");
                            Thread.Sleep(200);

                        }
                    }
                }
                clockCycle++;
            }
        }
    }

    public class AssignTasksToProcessors : ITasksPriorityHandler
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

    public class Test
    {
        public void TasksToProcessors(TaskList taskList, TasksQueue tasksQueue, List<Processor> processors, ref int clockCycle)
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
                            ConsoleStyler.SetTextColor(ConsoleColor.Green);
                            Console.WriteLine($"|-- Task [{processor.CurrentTask.Id}] Completed Successfully!");
                            ConsoleStyler.ResetTextColor();
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
                            Console.WriteLine($"|-- Task [{task.Id}] is assigned to Processor [{processor.Id}] at clock cyclc ===> {clockCycle}");
                            Thread.Sleep(100);
                        }
                        else if (tasksQueue.LowPriorityTasks.Count > 0)
                        {
                            Task task = tasksQueue.LowPriorityTasks.Dequeue();
                            processor.CurrentTask = task;
                            processor.State = ProcessorState.BUSY;
                            task.State = TaskState.EXECUTING;
                            Console.WriteLine($"|-- Task [{task.Id}] is assigned to Processor [{processor.Id}] at clock cycle ===> {clockCycle}");
                            Thread.Sleep(100);
                        }
                    }
                }
                clockCycle++;
            }
        }
    }

    public class OutputData
    {
        public void PrintOutputData(List<Task> tasks, ref int clockCycle)
        {
            Console.WriteLine("\n---------------------------OUTPUT DATA---------------------------\n");
            ConsoleStyler.SetTextColor(ConsoleColor.Yellow);
            Console.WriteLine("Task ID | Creation Time | Completion Time | Priority | State");
            Console.WriteLine("--------|---------------|-----------------|----------|-----------");
            foreach (Task task in tasks)
            {
                Console.WriteLine($"{task.Id,-7} | {task.CreationTime,-13} | {task.CompletionTime,-15} | {task.Priority,-8} | {task.State}");
            }
            Console.WriteLine($"\nTotal Clock Cycles: {clockCycle}");
            ConsoleStyler.ResetTextColor();
        }
    }
}
