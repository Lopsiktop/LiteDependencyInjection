using System;
using System.Reflection;

namespace LDI.Models.Injection;

public  class InjectionBuilder
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

    public void AddTransient<I, T>()
        where I : class
        where T : class, I 
        => _AddService<I, T>(InjectionType.Transient);

    public void AddTransient<T>()
        where T : class
        => _AddService<T>(InjectionType.Transient);

    public I GetService<I>() => (I)GetService(typeof(I));

    private object _GetService(InjectionService service)
    {
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

    public object GetService(string typeName, bool ignoreCase = false)
    {
        var comparison = StringComparison.Ordinal;
        if (ignoreCase)
            comparison = StringComparison.OrdinalIgnoreCase;

        var interfaceService = _services.FirstOrDefault(x => x.Interface?.Name.Equals(typeName, comparison) ?? false);
        var classService = _services.FirstOrDefault(x => x.Realization.Name.Equals(typeName, comparison));

        if (interfaceService is not null)
            return _GetService(interfaceService);
        else if (classService is not null)
            return _GetService(classService);

        throw new Exception($"Non existent service {typeName}");
    }

    public object GetService(Type type) => GetService(type.Name);
}
