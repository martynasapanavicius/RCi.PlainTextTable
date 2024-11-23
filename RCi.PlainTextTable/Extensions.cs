using System.Collections.Immutable;

namespace RCi.PlainTextTable
{
    public static class Extensions
    {
        internal static ImmutableArray<T> UnsafeAsImmutableArray<T>(this T[] src) =>
            System.Runtime.CompilerServices.Unsafe.As<T[], ImmutableArray<T>>(ref src);
    }
}
