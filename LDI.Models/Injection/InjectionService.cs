using System.Reflection;

namespace LDI.Models.Injection;

internal class InjectionService
{
    public InjectionService(Type _interface, Type realization, ConstructorInfo constructorInfo, InjectionType type)
    {
        Interface = _interface;
        Realization = realization;
        Type = type;
        ConstructorInfo = constructorInfo;
    }

    public Type Interface { get; set; }
    public Type Realization { get; set; }
    public InjectionType Type { get; set; } 
    public ConstructorInfo ConstructorInfo { get; set; }
}

internal enum InjectionType
{
    Singleton,
    Transient,
    Scoped
}