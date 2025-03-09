using LDI.Models.Injection;
using System.Windows;

namespace LDI.Extensions.WPF;

public static class WpfExtensions
{
    private const string VIEW_MODEL_PATTERN = "ViewModel";

    public static void RunWithLiteInjection<T>(this Application app, InjectionBuilder builder)
        where T: Window
    {
        var mainWindow = builder.GetService<T>();

        app.Run(mainWindow);
    }

    public static void RunWithMVVMInjection<T>(this Application app, InjectionBuilder builder)
        where T: Window
    {
        var mainWindow = builder.GetWindow<T>();

        app.Run(mainWindow);
    }

    public static T GetWindow<T>(this InjectionBuilder builder)
        where T: Window
    {
        var windowType = typeof(T);
        var viewModelName = windowType.Name + VIEW_MODEL_PATTERN;

        var viewModelService = builder.GetService(viewModelName, true);
        var windowService = builder.GetService<T>();

        windowService.DataContext = viewModelService;

        return windowService;
    }
}