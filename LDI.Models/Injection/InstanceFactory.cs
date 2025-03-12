namespace LDI.Models.Injection;

internal class InstanceFactory
{
    private static List<object> singletons = new List<object>();

    public object? GetInstance(InjectionService service) => GetInstance(service, null);

    public object? GetInstance(InjectionService service, object[]? args)
    {
        if (service.Type == InjectionType.Transient)
            return Activator.CreateInstance(service.Realization, args);
        else if (service.Type == InjectionType.Singleton)
        {
            var singleton = singletons.FirstOrDefault(x => x!.GetType() == service.Realization, null);
            if (singleton is not null)
                return singleton;

            var realization = Activator.CreateInstance(service.Realization, args)!;
            singletons.Add(realization);

            return realization;
        }

        throw new Exception("Unsupported injection type");
    }
}