namespace CPU
{
    public class Task
    {
        public string? Id { get; set; }
        public int CreationTime { get; set; }
        public int RequestedTime { get; set; }
        public int CompletionTime { get; set; }
        public string? Priority { get; set; }
        public TaskState State { get; set; }
    }
}