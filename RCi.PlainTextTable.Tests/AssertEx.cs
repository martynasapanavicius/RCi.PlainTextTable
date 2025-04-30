using System;

namespace RCi.Toolbox.Ptt.Tests
{
    public static class AssertEx
    {
        internal static string ToUnixLineEnding(this string input) => input.Replace("\r\n", "\n").Replace('\r', '\n');

        internal static string[] SplitLines(this string input, StringSplitOptions options = StringSplitOptions.None) =>
            options.HasFlag(StringSplitOptions.RemoveEmptyEntries)
                ? input.Split(["\r\n", "\r", "\n"], options)
                : input.ToUnixLineEnding().Split('\n', options);

        public static void That(string actual, string expected)
        {
            var actualUnix = actual.ToUnixLineEnding();
            var expectedUnix = actual.ToUnixLineEnding();
            Assert.That(actualUnix, Is.EqualTo(expectedUnix));
        }

        public static void That(char actual, char expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
