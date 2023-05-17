namespace CPU
{
    public class SortByCreationTime : TaskList
    {
        public SortByCreationTime(List<Task> tasks) : base(tasks) { }
        public override void SortTasks()
        {
            Tasks = Tasks.OrderBy(task => task.CreationTime).ToList();
        }
    }
}