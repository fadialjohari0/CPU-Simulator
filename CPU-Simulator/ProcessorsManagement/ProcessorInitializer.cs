namespace CPU
{
    public class ProcessorInitializer : IProcessorInitializer
    {
        List<Processor> processors = new List<Processor>();
        public List<Processor> InitializeProcessors(int numOfProcessors)
        {
            for (int i = 0; i < numOfProcessors; i++)
            {
                Processor processor = new Processor()
                {
                    Id = $"P{i + 1}",
                    State = ProcessorState.IDLE
                };
                processors.Add(processor);
            }
            return processors;
        }
    }
}