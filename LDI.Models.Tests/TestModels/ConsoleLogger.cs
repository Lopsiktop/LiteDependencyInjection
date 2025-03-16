namespace LDI.Models.Tests
{
    public class ConsoleLogger : ILogger
    {
        public string Name { get; set; }

        public ConsoleLogger()
        {
            Name = Guid.NewGuid().ToString();
        }

        public void Send(string msg)
        {
            Console.Write(msg);
        }
    }
}