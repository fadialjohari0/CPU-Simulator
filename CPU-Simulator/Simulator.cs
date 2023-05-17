using Newtonsoft.Json;

namespace CPU
{
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
}