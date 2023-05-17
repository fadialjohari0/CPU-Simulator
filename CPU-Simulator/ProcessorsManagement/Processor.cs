namespace CPU
{
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
}