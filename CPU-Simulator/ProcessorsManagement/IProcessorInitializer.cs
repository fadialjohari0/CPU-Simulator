namespace CPU
{
    public interface IProcessorInitializer
    {
        List<Processor> InitializeProcessors(int numOfProcessors);
    }
}