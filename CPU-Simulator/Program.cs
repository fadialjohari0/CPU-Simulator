using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CPU
{
    class Program
    {
        static void Main(string[] args)
        {
            IProcessorInitializer processorInitializer = new ProcessorInitializer();
            ITasksPriorityHandler tasksPriorityHandler = new TasksPriorityHandler();
            Scheduler scheduler = new Scheduler();
            OutputFile outputFile = new OutputFile();

            Simulator simulator = new Simulator(processorInitializer, tasksPriorityHandler, scheduler, outputFile);
            simulator.Start();
        }
    }
}