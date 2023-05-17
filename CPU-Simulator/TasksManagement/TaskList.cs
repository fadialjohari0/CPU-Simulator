namespace CPU
{
    public abstract class TaskList
    {
        public List<Task> Tasks { get; protected set; }
        public TaskList(List<Task> tasks)
        {
            Tasks = tasks;
        }
        public abstract void SortTasks();
    }
}