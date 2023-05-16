namespace CPU
{
    interface ITasksPriorityHandler
    {
        public void SeparateTasksByPriority(TaskList taskList, TasksQueue tasksQueue, ref int clockCycle);
    }
}