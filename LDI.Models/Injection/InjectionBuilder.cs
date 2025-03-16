namespace LDI.Models.Injection;

public class InjectionBuilder : InjectionProvider
{
    private readonly List<InjectionService> _services;
    private readonly List<object> _singletons;

    public InjectionBuilder()
    {
        _services = new List<InjectionService>();
        _singletons = new List<object>();
    }

    private void _ExceptionValidation(Type typeT, Type typeI)
    {
        _InterfaceExceptionValidation(typeI);

        _RealizationExceptionValidation(typeT);

        var implemented = typeT.GetInterfaces().Contains(typeI);
        if (!implemented)
            throw new NotImplementedException($"Type {typeT.Name} has to be implemented by {typeI.Name}");
    }

    private void _InterfaceExceptionValidation(Type typeI)
    {
        if (!typeI.IsInterface)
            throw new Exception($"You can pass only interfaces, not {typeI.Name}");

        var contains = _services.FirstOrDefault(x => x.Interface == typeI);
        if (contains is not null)
            throw new Exception($"Interface {typeI.Name} already in use");
    }

    private void _RealizationExceptionValidation(Type typeT)
    {
        var contains = _services.FirstOrDefault(x => x.Realization == typeT);
        if (contains is not null)
            throw new Exception($"Class {typeT.Name} already in use");

        if (!typeT.IsClass)
            throw new Exception($"You can pass only classes, not {typeT.Name}");

        if (typeT.Name == nameof(String))
            throw new Exception($"You cannot pass String");

        if (typeT.IsAbstract)
            throw new Exception($"You cannot pass abstract classes like {typeT.Name}");
    }

    public ScopeProvider CreateScope() => new ScopeProvider(this, _services, _singletons);

    private void _AddService<I, T>(InjectionType injtype)
        where I : class
        where T : class, I
    {
        Type typeT = typeof(T);
        Type typeI = typeof(I);

        _ExceptionValidation(typeT, typeI);

        var service = new InjectionService(typeI, typeT, injtype);
        _services.Add(service);
    }

    private void _AddService<T>(InjectionType injtype)
        where T : class
    {
        Type type = typeof(T);

        _RealizationExceptionValidation(type);

        var service = new InjectionService(null, type, injtype);
        _services.Add(service);
    }

    public void AddScoped<I, T>()
        where I : class
        where T : class, I
        => _AddService<I, T>(InjectionType.Scoped);

    public void AddScoped<T>()
        where T : class
        => _AddService<T>(InjectionType.Scoped);

    public void AddSingleton<I, T>()
        where I : class
        where T : class, I
        => _AddService<I, T>(InjectionType.Singleton);

    public void AddSingleton<T>()
        where T : class
        => _AddService<T>(InjectionType.Singleton);

    public void AddTransient<I, T>()
        where I : class
        where T : class, I 
        => _AddService<I, T>(InjectionType.Transient);

    public void AddTransient<T>()
        where T : class
        => _AddService<T>(InjectionType.Transient);

    public override object GetService(string typeName, bool ignoreCase = false)
    {
        var comparison = StringComparison.Ordinal;
        if (ignoreCase)
            comparison = StringComparison.OrdinalIgnoreCase;

        var interfaceService = _services.FirstOrDefault(x => x.Interface?.Name.Equals(typeName, comparison) ?? false);
        var classService = _services.FirstOrDefault(x => x.Realization.Name.Equals(typeName, comparison));

        if (interfaceService is not null)
            return GetService(interfaceService, _singletons);
        else if (classService is not null)
            return GetService(classService, _singletons);

        throw new Exception($"Non existent service {typeName}");
    }
}
