namespace LDI.Models.Tests
{
    public class ConsoleLogger : ILogger
    {
        public void Send(string msg)
        {
            Console.Write(msg);
        }
    }
}