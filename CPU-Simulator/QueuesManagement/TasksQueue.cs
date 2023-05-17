namespace CPU
{
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
}