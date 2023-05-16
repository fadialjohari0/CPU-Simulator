namespace CPU
{
    public class AssigningTasksToProcessors
    {
        public void AssignTasksToProcessors(TaskList taskList, TasksQueue tasksQueue, List<Processor> processors, ref int clockCycle)
        {
            while (tasksQueue.HighPriorityTasks.Count != 0 || tasksQueue.LowPriorityTasks.Count != 0 || processors.Any(processor => processor.State == ProcessorState.BUSY))
            {
                foreach (Processor processor in processors)
                {
                    if (processor.State == ProcessorState.BUSY)
                    {
                        processor.CurrentTask!.RequestedTime--;
                        if (processor.CurrentTask.RequestedTime == 0)
                        {
                            processor.CurrentTask.State = TaskState.COMPLETED;
                            processor.State = ProcessorState.IDLE;
                            processor.CurrentTask.CompletionTime = clockCycle;
                            ConsoleStyler.SetTextColor(ConsoleColor.Green);
                            Console.WriteLine($"|-- Task [{processor.CurrentTask.Id}] Completed Successfully!");
                            ConsoleStyler.ResetTextColor();
                            processor.CurrentTask = null;

                        }
                    }
                    else if (processor.State == ProcessorState.IDLE)
                    {
                        if (tasksQueue.HighPriorityTasks.Count > 0)
                        {
                            Task task = tasksQueue.HighPriorityTasks.Dequeue();
                            processor.CurrentTask = task;
                            processor.State = ProcessorState.BUSY;
                            task.State = TaskState.EXECUTING;
                            Console.WriteLine($"|-- Task [{task.Id}] is assigned to Processor [{processor.Id}] at clock cyclc --> {clockCycle}");
                            Thread.Sleep(100);
                        }
                        else if (tasksQueue.LowPriorityTasks.Count > 0)
                        {
                            Task task = tasksQueue.LowPriorityTasks.Dequeue();
                            processor.CurrentTask = task;
                            processor.State = ProcessorState.BUSY;
                            task.State = TaskState.EXECUTING;
                            Console.WriteLine($"|-- Task [{task.Id}] is assigned to Processor [{processor.Id}] at clock cycle --> {clockCycle}");
                            Thread.Sleep(100);
                        }
                    }
                }
                clockCycle++;
            }
        }
    }
}