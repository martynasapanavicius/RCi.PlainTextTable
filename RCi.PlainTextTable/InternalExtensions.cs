using System.Collections.Immutable;

namespace RCi.Toolbox
{
    internal static class InternalExtensions
    {
        public static ImmutableArray<T> UnsafeAsImmutableArray<T>(this T[] src) =>
            System.Runtime.CompilerServices.Unsafe.As<T[], ImmutableArray<T>>(ref src);
    }
}
