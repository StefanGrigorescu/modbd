using System.Diagnostics.CodeAnalysis;

namespace MODBD_Api.Common.Abstractions.DiscriminatedUnions;

public static class DiscriminatedUnionsExtensions
{
    public static TResult Match<TBase, TDerived1, TDerived2, TType, TResult>(
        this TBase src,
        Func<TDerived1, TResult> functionIfIsDerived1,
        Func<TDerived2, TResult> functionIfIsDerived2,
        Func<TBase, TResult> defaultFunctionIfIsNotMatched
    )
        where TBase : IOfEitherType<TType>
        where TType : Enumeration<TType>
        where TDerived1 : TBase, IOfType<TType>
        where TDerived2 : TBase, IOfType<TType>
    {
        if (src.TryCast<TBase, TDerived1, TType>(out TDerived1? dest1))
        {
            return functionIfIsDerived1(dest1);
        }

        if (src.TryCast<TBase, TDerived2, TType>(out TDerived2? dest2))
        {
            return functionIfIsDerived2(dest2);
        }

        return defaultFunctionIfIsNotMatched(src);
    }

    public static TResult Match<TBase, TDerived1, TDerived2, TDerived3, TType, TResult>(
        this TBase src,
        Func<TDerived1, TResult> functionIfIsTDerived1,
        Func<TDerived2, TResult> functionIfIsTDerived2,
        Func<TDerived3, TResult> functionIfIsTDerived3,
        Func<TBase, TResult> defaultFunctionIfIsNotMatched
    )
        where TBase : IOfEitherType<TType>
        where TType : Enumeration<TType>
        where TDerived1 : TBase, IOfType<TType>
        where TDerived2 : TBase, IOfType<TType>
        where TDerived3 : TBase, IOfType<TType>
    {
        if (src.TryCast<TBase, TDerived1, TType>(out TDerived1? dest1))
        {
            return functionIfIsTDerived1(dest1);
        }

        if (src.TryCast<TBase, TDerived2, TType>(out TDerived2? dest2))
        {
            return functionIfIsTDerived2(dest2);
        }

        if (src.TryCast<TBase, TDerived3, TType>(out TDerived3? dest3))
        {
            return functionIfIsTDerived3(dest3);
        }

        return defaultFunctionIfIsNotMatched(src);
    }

    public static void Match<TBase, TDerived1, TDerived2, TType>(
        this TBase src,
        Action<TDerived1> actionIfIsTDerived1,
        Action<TDerived2> actionIfIsTDerived2,
        Action<TBase>? defaultActionIfIsNotMatched = null
    )
        where TBase : IOfEitherType<TType>
        where TType : Enumeration<TType>
        where TDerived1 : TBase, IOfType<TType>
        where TDerived2 : TBase, IOfType<TType>
    {
        if (src.TryCast<TBase, TDerived1, TType>(out TDerived1? dest1))
        {
            actionIfIsTDerived1(dest1);
            return;
        }

        if (src.TryCast<TBase, TDerived2, TType>(out TDerived2? dest2))
        {
            actionIfIsTDerived2(dest2);
            return;
        }

        if (defaultActionIfIsNotMatched is not null)
        {
            defaultActionIfIsNotMatched(src);
        }
    }

    public static void Match<TBase, TDerived1, TDerived2, TDerived3, TType>(
        this TBase src,
        Action<TDerived1> actionIfIsTDerived1,
        Action<TDerived2> actionIfIsTDerived2,
        Action<TDerived3> actionIfIsTDerived3,
        Action<TBase>? defaultActionIfIsNotMatched = null
    )
        where TBase : IOfEitherType<TType>
        where TType : Enumeration<TType>
        where TDerived1 : TBase, IOfType<TType>
        where TDerived2 : TBase, IOfType<TType>
        where TDerived3 : TBase, IOfType<TType>
    {
        if (src.TryCast<TBase, TDerived1, TType>(out TDerived1? dest1))
        {
            actionIfIsTDerived1(dest1);
            return;
        }

        if (src.TryCast<TBase, TDerived2, TType>(out TDerived2? dest2))
        {
            actionIfIsTDerived2(dest2);
            return;
        }

        if (src.TryCast<TBase, TDerived3, TType>(out TDerived3? dest3))
        {
            actionIfIsTDerived3(dest3);
            return;
        }

        if (defaultActionIfIsNotMatched is not null)
        {
            defaultActionIfIsNotMatched(src);
        }
    }

    public static bool TryCast<TBase, TDerived, TType>(
           this TBase src,
           [NotNullWhen(true)] out TDerived? dest
    )
        where TBase : IOfEitherType<TType>
        where TDerived : TBase, IOfType<TType>
        where TType : Enumeration<TType>
    {
        if (src.Type == TDerived.ClassType)
        {
            dest = (TDerived)src;
            return true;
        }

        dest = default;
        return false;
    }
}
