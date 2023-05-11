using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CPU
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter number of processors: ");
            int numOfProcessors = int.Parse(Console.ReadLine());

            List<Processor> listOfProcessors = new List<Processor>();

            for (int i = 0; i < numOfProcessors; i++)
            {
                Processor processor = new Processor()
                {
                    Id = $"P{i + 1}",
                    State = ProcessorState.IDLE
                };
                listOfProcessors.Add(processor);
            }




            /***************************************************/

            List<Task> listOfTasks = new List<Task>();

            Queue<Task> HighPriorityTasks = new Queue<Task>();
            Queue<Task> LowPriorityTasks = new Queue<Task>();

            string fileContent = File.ReadAllText("Tasks.Json");
            JArray jsonArray = JArray.Parse(fileContent);

            foreach (JObject obj in jsonArray)
            {
                string id = obj["Id"]!.ToString();
                int creationTime = int.Parse(obj["CreationTime"]!.ToString());
                int requestedTime = int.Parse(obj["RequestedTime"]!.ToString());
                string priority = obj["Priority"]!.ToString();

                Task task = new Task()
                {
                    Id = id,
                    CreationTime = creationTime,
                    RequestedTime = requestedTime,
                    Priority = priority
                };

                listOfTasks.Add(task);
            }

            foreach (Task task in listOfTasks)
            {
                if (task.Priority == "High")
                    HighPriorityTasks.Enqueue(task);
                else
                    LowPriorityTasks.Enqueue(task);
            }
            foreach (var n in HighPriorityTasks)
            {

                foreach (Processor processor1 in listOfProcessors)
                {
                    if (processor1.State == ProcessorState.IDLE)
                    {
                        processor1.AssignTask(n);
                        break;
                    }
                }
            }

            foreach (var b in listOfProcessors)
            {
                Console.WriteLine(b.State);
            }

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
        public Task CurrentTask { get; set; }

        public void AssignTask(Task task)
        {
            CurrentTask = task;
            State = ProcessorState.BUSY;
        }
    }

    public class Scheduler
    {

    }

    public class Clock
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