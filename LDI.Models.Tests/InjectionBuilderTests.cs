using FluentAssertions;
using LDI.Models.Injection;
using LDI.Models.Tests.TestModels;
using System.Reflection;

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
        public void GetService_AddTransientClassService_ReturnsEmailSender()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ILogger, ConsoleLogger>();
            builder.AddTransient<EmailSender>();

            // act
            var sender = builder.GetService<EmailSender>();

            // assert
            sender.Should().NotBeNull();
        }

        [Fact]
        public void GetServiceTypeName_WithExistentService_ReturnsConsoleLogger()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ConsoleLogger>();

            // act
            var sender = builder.GetService("ConsoleLogger");

            // assert
            sender.Should().NotBeNull();
            sender.Should().BeOfType<ConsoleLogger>();
        }

        [Fact]
        public void GetServiceTypeName_WithExistentServiceButWrongString_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ConsoleLogger>();

            // act
            Action act = () => builder.GetService("consolelogger");

            // assert
            act.Should().Throw<Exception>().WithMessage("Non existent service consolelogger");
        }

        [Fact]
        public void GetServiceTypeName_WithExistentServiceAndIgnoreCase_ReturnsConsoleLogger()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ConsoleLogger>();

            // act
            var result = builder.GetService("consoleLOGGER", true);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ConsoleLogger>();
        }

        [Fact]
        public void GetServiceByType_WithExistentService_ReturnsConsoleLogger()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ConsoleLogger>();

            // act
            var result = builder.GetService(typeof(ConsoleLogger));

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ConsoleLogger>();
        }

        [Fact]
        public void GetServiceByType_WithNonExistentService_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();
            builder.AddTransient<ConsoleLogger>();

            // act
            Action act = () => builder.GetService(typeof(string));

            // assert
            act.Should().Throw<Exception>().WithMessage("Non existent service String");
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

        [Fact]
        public void AddTransient_WithPrimitiveType_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();

            // act
            Action act = () => builder.AddTransient<string>();

            // assert
            act.Should().Throw<Exception>().WithMessage("You cannot pass String");
        }

        [Fact]
        public void AddTransient_WithTheSameClasses_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();

            // act
            Action act = () => builder.AddTransient<ConsoleLogger, ConsoleLogger>();

            // assert
            act.Should().Throw<Exception>().WithMessage("You can pass only interfaces, not ConsoleLogger");
        }

        [Fact]
        public void AddTransient_WithAbstractClassInFirstPattern_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();

            // act
            Action act = () => builder.AddTransient<AbstractLogger, NewLogger>();

            // assert
            act.Should().Throw<Exception>().WithMessage("You can pass only interfaces, not AbstractLogger");
        }

        [Fact]
        public void AddTransientSingle_WithAbstractClass_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();

            // act
            Action act = () => builder.AddTransient<AbstractLogger>();

            // assert
            act.Should().Throw<Exception>().WithMessage("You cannot pass abstract classes like AbstractLogger");
        }

        [Fact]
        public void AddTransientSingle_WithInterface_ThrowsException()
        {
            // arrange
            var builder = new InjectionBuilder();

            // act
            Action act = () => builder.AddTransient<ILogger>();

            // assert
            act.Should().Throw<Exception>().WithMessage("You can pass only classes, not ILogger");
        }
    }
}