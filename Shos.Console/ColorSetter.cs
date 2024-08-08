namespace Shos.Console
{
    using System;

    /// <summary>A class that temporarily sets the console foreground and background colors.</summary>
    public class ColorSetter : IDisposable
    {
        readonly ConsoleColor currentForegroundColor = Console.ForegroundColor;
        readonly ConsoleColor currentBackgroundColor = Console.BackgroundColor;

        /// <summary>Initializes a new instance of the <see cref="ColorSetter"/> class and sets the console colors.</summary>
        /// <param name="foregroundColor">The foreground color to set.</param>
        /// <param name="backgroundColor">The background color to set.</param>
        public ColorSetter(ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            (currentForegroundColor, currentBackgroundColor) = (Console.ForegroundColor, Console.BackgroundColor);
            (Console.ForegroundColor, Console.BackgroundColor) = (foregroundColor, backgroundColor);
        }

        /// <summary>Restores the console colors to their original values.</summary>
        public void Dispose()
            => (Console.ForegroundColor, Console.BackgroundColor) = (currentForegroundColor, currentBackgroundColor);
    }
}
