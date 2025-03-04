using System.Reflection;

namespace LDI.Models.Injection;

public class InjectionBuilder
{
    private readonly List<InjectionService> _services;
    public InjectionBuilder()
    {
        _services = new List<InjectionService>();
    }

    public void AddTransient<I, T>()
        where I: class
        where T: class, new()
    {
        Type typeT = typeof(T);
        Type typeI = typeof(I);

        var implemented = typeT.GetInterfaces().Contains(typeI);
        if (!implemented)
            throw new NotImplementedException($"Type {typeT.Name} has to be implemented by {typeI.Name}");


    }
}
