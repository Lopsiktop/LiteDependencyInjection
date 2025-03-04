using LDI.Models.Attributes;
using System;
using System.Reflection;

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

    private ConstructorInfo _EcxeptionValidation(Type typeT, Type typeI)
    {
        var implemented = typeT.GetInterfaces().Contains(typeI);
        if (!implemented)
            throw new NotImplementedException($"Type {typeT.Name} has to be implemented by {typeI.Name}");
        var constructors = typeT.GetConstructors().Where(x => x.IsPublic).ToArray();
        ConstructorInfo constructor = null;

        if (constructors.Length == 1)
            constructor = constructors.First();
        if (constructors.Length > 1)
        {
            var attrsConstructor = constructors.Where(x => x.CustomAttributes.Any(c => c.AttributeType == typeof(InjectionConstructorAttribute)));
            if (attrsConstructor.Count() == 0)
                throw new Exception($"Class {typeT.Name} has more than 1 constructor. For using specific constructor use attribute {nameof(InjectionConstructorAttribute)}");
            else if (attrsConstructor.Count() == 1)
                constructor = attrsConstructor.First();
            else
                throw new Exception($"Class {typeT.Name} cannot have more than 1 {nameof(InjectionConstructorAttribute)} attribute");
        }

        return constructor!;
    }

    public void AddTransient<I, T>()
        where I : class
        where T : class, I
    {
        Type typeT = typeof(T);
        Type typeI = typeof(I);

        var constructor = _EcxeptionValidation(typeT, typeI);

        var service = new InjectionService(typeI, typeT, constructor, InjectionType.Transient);
        _services.Add(service);
    }

    public I GetService<I>() => (I)GetService(typeof(I));

    public object GetService(Type type)
    {
        var service = _services.FirstOrDefault(x => x.Interface == type);

        if (service is null)
            throw new Exception($"Non existent service {type.Name}");

        var parameters = service.ConstructorInfo.GetParameters();
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

        if (instance is null)
            throw new ArgumentNullException($"Something went wrong trying to create {service.Realization.Name}");

        return instance;
    }
}
