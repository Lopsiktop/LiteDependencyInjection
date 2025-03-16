namespace LDI.Models.Injection;

public class ScopeProvider : InjectionProvider, IDisposable
{
    private readonly InjectionBuilder _builder;
    private readonly List<InjectionService> _scopedServices;
    private readonly List<object> _singletons;

    private bool _disposed = false;

    internal ScopeProvider(InjectionBuilder builder,
        List<InjectionService> services,
        List<object> singletons)
    {
        _builder = builder;

        _scopedServices = services
            .Where(x => x.Type == InjectionType.Scoped)
            .Select(x => x.Clone(InjectionType.Singleton))
            .ToList();

        _singletons = singletons.ToList();
    }

    public override object GetService(string typeName, bool ignoreCase = false)
    {
        if (_disposed)
            throw new Exception("This scope is already disposed");

        // надо рефакторинг провести, избавиться от повторяющегося кода (включая код GetService из InjectionBuilder) 
        var comparison = StringComparison.Ordinal;
        if (ignoreCase)
            comparison = StringComparison.OrdinalIgnoreCase;

        var interfaceService = _scopedServices.FirstOrDefault(x => x.Interface?.Name.Equals(typeName, comparison) ?? false);
        var classService = _scopedServices.FirstOrDefault(x => x.Realization.Name.Equals(typeName, comparison));

        if (interfaceService is not null)
            return GetService(interfaceService, _singletons);
        else if (classService is not null)
            return GetService(classService, _singletons);

        return _builder.GetService(typeName, ignoreCase);
    }

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
