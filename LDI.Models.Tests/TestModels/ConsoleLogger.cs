namespace LDI.Models.Tests
{
    public class ConsoleLogger : ILogger
    {
        public string Name { get; set; }

        public ConsoleLogger()
        {
            Name = new Random().Next(5, 20).ToString();
        }

        public void Send(string msg)
        {
            Console.Write(msg);
        }
    }
}