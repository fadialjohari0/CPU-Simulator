namespace CPU
{
    public class PrintOutputData
    {
        public void PrintData(List<Task> tasks, ref int clockCycle)
        {
            Console.WriteLine("\n---------------------------OUTPUT DATA---------------------------\n");
            ConsoleStyler.SetTextColor(ConsoleColor.Yellow);
            Console.WriteLine("Task ID | Creation Time | Completion Time | Priority | State");
            Console.WriteLine("--------|---------------|-----------------|----------|-----------");
            foreach (Task task in tasks)
            {
                Console.WriteLine($"{task.Id,-7} | {task.CreationTime,-13} | {task.CompletionTime,-15} | {task.Priority,-8} | {task.State}");
            }
            Console.WriteLine($"\nTotal Clock Cycles: {clockCycle}");
            ConsoleStyler.ResetTextColor();
        }
    }
}