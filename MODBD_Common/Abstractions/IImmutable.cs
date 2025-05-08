using System.Reflection;
using MODBD_Common.Collections;

namespace MODBD_Common.Abstractions;

public interface IImmutable;

public interface IImmutable<TImmutable> : IImmutable, IEquatable<TImmutable> { }


public static class ImmutableTypes
{
    public static IReadOnlyList<Type> GetAllFromAssembly(Assembly assembly) =>
        assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IImmutable)))
            .ToIReadOnlyList();
}
