namespace CPU
{
    public class SortByRequestedTime : TaskList
    {
        public SortByRequestedTime(List<Task> tasks) : base(tasks) { }
        public override void SortTasks()
        {
            Tasks = Tasks.OrderBy(task => task.RequestedTime).ToList();
        }
    }
}