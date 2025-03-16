namespace LDI.Models.Injection;

internal static class InstanceFactory
{
    public static object? GetInstance(InjectionService service, List<object> singletons) => GetInstance(service, singletons, null);

    public static object? GetInstance(InjectionService service, 
        List<object> singletons,
        object[]? args)
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
        else if (service.Type == InjectionType.Scoped)
        {
            throw new Exception($"You cannot get scoped service ({service.Interface?.Name ?? service.Realization.Name}) from root builder");
        }

        throw new Exception("Unsupported injection type");
    }
}