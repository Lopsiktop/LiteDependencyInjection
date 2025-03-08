using FluentAssertions;
using LDI.Models.Injection;
using LDI.Models.Tests.TestModels;

namespace LDI.Models.Tests
{
    public class InjectionBuilderTests
    {
        [Fact]
        public void GetService_WithCorrectData_ReturnsEmailSender()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ILogger, ConsoleLogger>();
            builder.AddTransient<IEmailSender, EmailSender>();

            // act
            var sender = builder.GetService<IEmailSender>();

            // assert
            sender.Should().NotBeNull();
        }

        [Fact]
        public void AddTransient_WithAlreadyInUserInterface_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ILogger, ConsoleLogger>();
            builder.AddTransient<IEmailSender, EmailSender>();

            // act
            Action act = () => builder.AddTransient<ILogger, TxtLogger>();

            // assert
            act.Should().Throw<Exception>().WithMessage("Interface ILogger already in use");
        }

        [Fact]
        public void GetService_TryToGetNonExistentService_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();

            // act
            Action act = () => builder.GetService<ILogger>();

            // assert
            act.Should().Throw<Exception>().WithMessage("Non existent service ILogger");
        }
    }
}