namespace CPU
{
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
}