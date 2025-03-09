using LDI.Models.Attributes;
using System.Reflection;

namespace LDI.Models.Injection;

internal class InjectionService
{
    public Type? Interface { get; set; }
    public Type Realization { get; set; }
    public InjectionType Type { get; set; }

    public InjectionService(Type? _interface, Type realization, InjectionType type)
    {
        Interface = _interface;
        Realization = realization;
        Type = type;
    }

    public ConstructorInfo GetConstructor()
    {
        //todo: test this
        var typeT = Realization;
        var constructors = typeT.GetConstructors().Where(x => x.IsPublic).ToArray();
        ConstructorInfo constructor = null!;

        if (constructors.Length == 1)
            constructor = constructors.First();
        else if (constructors.Length > 1)
        {
            var attrsConstructor = constructors.Where(x => x.CustomAttributes.Any(c => c.AttributeType == typeof(InjectionConstructorAttribute)));
            if (attrsConstructor.Count() == 0)
                throw new Exception($"Class {typeT.Name} has more than 1 constructor. For using specific constructor use attribute {nameof(InjectionConstructorAttribute)}");
            else if (attrsConstructor.Count() == 1)
                constructor = attrsConstructor.First();
            else
                throw new Exception($"Class {typeT.Name} cannot have more than 1 {nameof(InjectionConstructorAttribute)} attribute");
        }

        return constructor;
    }
}
