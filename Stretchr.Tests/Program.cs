using System;
using System.Reflection;
using NUnit.ConsoleRunner;

namespace Stretchr.Tests
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            string[] my_args = {Assembly.GetExecutingAssembly().Location};

            int returnCode = Runner.Main(my_args);

            if (returnCode != 0)
                Console.Beep();
        }
    }
}