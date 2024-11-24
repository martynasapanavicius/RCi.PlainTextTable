using System.Collections.Immutable;

namespace RCi.PlainTextTable
{
    internal static class InternalExtensions
    {
        public static ImmutableArray<T> UnsafeAsImmutableArray<T>(this T[] src) =>
            System.Runtime.CompilerServices.Unsafe.As<T[], ImmutableArray<T>>(ref src);
    }
}
