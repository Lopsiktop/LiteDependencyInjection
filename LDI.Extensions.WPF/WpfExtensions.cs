using LDI.Models.Injection;
using System.Windows;

namespace LDI.Extensions.WPF;

public static class WpfExtensions
{
    public static void RunWithLiteInjection<T>(this Application app, InjectionBuilder builder)
        where T: Window
    {
        var mainWindow = builder.GetService<T>();

        app.Run(mainWindow);
    }
}
