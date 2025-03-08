namespace LDI.Models.Injection;

public class InjectionBuilder
{
    private readonly List<InjectionService> _services;
    private readonly InstanceFactory _factory;
    public InjectionBuilder()
    {
        _services = new List<InjectionService>();
        _factory = new InstanceFactory();
    }

    private void _ExceptionValidation(Type typeT, Type typeI)
    {
        var implemented = typeT.GetInterfaces().Contains(typeI);
        if (!implemented)
            throw new NotImplementedException($"Type {typeT.Name} has to be implemented by {typeI.Name}");

        var contains = _services.FirstOrDefault(x => x.Interface == typeI);
        if (contains is not null)
            throw new Exception($"Interface {typeI.Name} already in use");
    }

    public void AddTransient<I, T>()
        where I : class
        where T : class, I
    {
        Type typeT = typeof(T);
        Type typeI = typeof(I);

        _ExceptionValidation(typeT, typeI);

        var service = new InjectionService(typeI, typeT, InjectionType.Transient);
        _services.Add(service);
    }

    public I GetService<I>() => (I)GetService(typeof(I));

    public object GetService(Type type)
    {
        var service = _services.FirstOrDefault(x => x.Interface == type);

        if (service is null)
            throw new Exception($"Non existent service {type.Name}");

        //refactor this
        var parameters = service.GetConstructor().GetParameters();
        object? instance = null;

        if (parameters.Length == 0)
            instance = _factory.GetInstance(service);
        else
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var ptype = parameters[i].ParameterType;
                args[i] = GetService(ptype);
            }

            instance = _factory.GetInstance(service, args);
        }
        //end refactor

        if (instance is null)
            throw new ArgumentNullException($"Something went wrong trying to create {service.Realization.Name}");

        return instance;
    }
}
