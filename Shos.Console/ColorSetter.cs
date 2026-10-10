namespace Shos.Console;

using System;

/// <summary>A class that temporarily sets the console foreground and background colors.</summary>
/// <param name="foregroundColor">The foreground color to set.</param>
/// <param name="backgroundColor">The background color to set.</param>
public class ColorSetter(ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black) : IDisposable
{
    readonly (ConsoleColor foregroundColor, ConsoleColor backgroundColor) originalColors = SetColors(foregroundColor, backgroundColor);

    /// <summary>Restores the console colors to their original values.</summary>
    public void Dispose() => SetColors(originalColors.foregroundColor, originalColors.backgroundColor);

    /// <summary>Sets the console colors.</summary>
    /// <param name="foregroundColor">The foreground color to set.</param>
    /// <param name="backgroundColor">The background color to set.</param>
    /// <returns>The console colors before they were set.</returns>
    static (ConsoleColor foregroundColor, ConsoleColor backgroundColor) SetColors(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {
        var originalColors = (Console.ForegroundColor, Console.BackgroundColor);
        (Console.ForegroundColor, Console.BackgroundColor) = (foregroundColor, backgroundColor);
        return originalColors;
    }
}
