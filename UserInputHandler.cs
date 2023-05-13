interface IUserInputHandler
{
    int GetNumOfProcessorsFromUser();
}

public class UserInputHandler : IUserInputHandler
{
    public int GetNumOfProcessorsFromUser()
    {
        Console.Write("Enter number of processors: ");
        int numOfProcessors;
        while (!int.TryParse(Console.ReadLine(), out numOfProcessors))
        {
            Console.WriteLine("Invalid input. Please enter a positive integer.");
            Console.Write("Enter number of processors: ");
        }
        return numOfProcessors;
    }
}
