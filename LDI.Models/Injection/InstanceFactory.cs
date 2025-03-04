namespace LDI.Models.Injection;

internal class InstanceFactory
{
    private List<object> sigletons;

    public InstanceFactory()
    {
        sigletons = new List<object>();
    }

    public object? GetInstance(InjectionService service) => GetInstance(service, null);

    public object? GetInstance(InjectionService service, object[]? args)
    {
        if (service.Type == InjectionType.Transient)
            return Activator.CreateInstance(service.Realization, args);

        throw new Exception("Unsupported injection type");
    }
}