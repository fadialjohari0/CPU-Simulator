namespace CPU
{
    public class AssigningTasksToQueues : ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle)
        {
            while (taskList.Tasks.Count != (tasksQueue.HighPriorityTasks.Count + tasksQueue.LowPriorityTasks.Count))
            {
                foreach (Task task in taskList.Tasks)
                {
                    if (task.CreationTime == clockCycle)
                    {
                        if (task.Priority == "High")
                        {
                            tasksQueue.HighPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                            Console.WriteLine($"Task [{task.Id}] is added to the Queue at clock cycle: [{clockCycle}]");
                            Thread.Sleep(200);

                        }
                        else if (task.Priority == "Low")
                        {
                            tasksQueue.LowPriorityTasks.Enqueue(task);
                            task.State = TaskState.WAITING;
                            Console.WriteLine($"Task [{task.Id}] is added to the Queue at clock cycle: [{clockCycle}]");
                            Thread.Sleep(200);

                        }
                    }
                }
                clockCycle++;
            }
        }
    }
}