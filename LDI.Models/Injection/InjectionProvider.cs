namespace LDI.Models.Injection;

public abstract class InjectionProvider
{
    public abstract object GetService(string typeName, bool ignoreCase = false);

    public I GetService<I>() => (I)GetService(typeof(I));

    public object GetService(Type type) => GetService(type.Name);

    internal object GetService(InjectionService service, List<object> singletons)
    {
        var parameters = service.GetConstructor().GetParameters();
        object? instance = null;

        if (parameters.Length == 0)
            instance = InstanceFactory.GetInstance(service, singletons);
        else
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var ptype = parameters[i].ParameterType;
                args[i] = GetService(ptype);
            }

            instance = InstanceFactory.GetInstance(service, singletons, args);
        }

        if (instance is null)
            throw new ArgumentNullException($"Something went wrong trying to create {service.Realization.Name}");

        return instance;
    }
}