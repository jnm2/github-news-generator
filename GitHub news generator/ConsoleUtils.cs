using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GitHubNewsGenerator
{
    public static class ConsoleUtils
    {
        public static char Choose(string message)
        {
            var options = Regex.Matches(message, @"\[(?<char>[^\[\]])\](?<rest>[\w ]+)");
            if (options.Count < 2)
                throw new ArgumentException("At least two options must be specified.", nameof(message));

            var fullTextByOptionChar = new Dictionary<char, string>();

            foreach (Match option in options)
            {
                var c = message[option.Groups["char"].Index];
                var fullText = c + option.Groups["rest"].Value;

                if (!fullTextByOptionChar.TryAdd(c, fullText))
                {
                    throw new ArgumentException("Each option must be a unique character.", nameof(message));
                }
            }

            DisplayChoice(message, options);

            var chosen = '\0';
            var currentText = string.Empty;

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                    case ConsoleKey.Escape:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.Home:
                        if (chosen != '\0')
                        {
                            ReplaceText(currentText.Length, null);
                            chosen = '\0';
                            currentText = string.Empty;
                        }
                        else
                        {
                            Console.Beep();
                        }
                        break;

                    case ConsoleKey.Enter:
                        if (chosen != '\0')
                        {
                            Console.WriteLine();
                            return chosen;
                        }
                        Console.Beep();
                        break;

                    default:
                        if (fullTextByOptionChar.TryGetValue(key.KeyChar, out var fullText))
                        {
                            ReplaceText(currentText.Length, fullText);
                            chosen = key.KeyChar;
                            currentText = fullText;
                        }
                        else
                        {
                            ReplaceText(currentText.Length, null);
                            Console.Beep();
                            chosen = '\0';
                            currentText = string.Empty;
                        }
                        break;
                }
            }
        }

        private static void ReplaceText(int currentLength, string newText)
        {
            newText ??= string.Empty;

            var lengthToErase = Math.Max(0, currentLength - newText.Length);
            var atomicBuffer = new char[lengthToErase * 2 + currentLength + newText.Length];

            for (var i = 0; i < lengthToErase; i++)
            {
                atomicBuffer[i] = '\b';
                atomicBuffer[lengthToErase + i] = ' ';
            }

            for (var i = 0; i < currentLength; i++)
            {
                atomicBuffer[lengthToErase * 2 + i] = '\b';
            }

            newText.CopyTo(0, atomicBuffer, lengthToErase * 2 + currentLength, newText.Length);

            Console.Write(atomicBuffer);
        }

        private static void DisplayChoice(string message, MatchCollection options)
        {
            var normal = (foreground: Console.ForegroundColor, background: Console.BackgroundColor);

            var hotForeground = normal.foreground switch
            {
                ConsoleColor.Gray => ConsoleColor.White
            };
            var bracketForeground = normal.foreground switch
            {
                ConsoleColor.Gray => ConsoleColor.DarkGray
            };
            var hotBackground = normal.background switch
            {
                ConsoleColor.Black => ConsoleColor.DarkBlue
            };

            var nextIndex = 0;
            foreach (Match option in options)
            {
                Console.Write(message.Substring(nextIndex, option.Index - nextIndex));

                Console.BackgroundColor = hotBackground;
                try
                {
                    Console.ForegroundColor = bracketForeground;
                    Console.Write('[');
                    Console.ForegroundColor = hotForeground;
                    Console.Write(message[option.Groups["char"].Index]);
                    Console.ForegroundColor = bracketForeground;
                    Console.Write(']');
                }
                finally
                {
                    (Console.ForegroundColor, Console.BackgroundColor) = normal;
                }

                nextIndex = option.Groups["rest"].Index;
            }

            Console.Write(message.Substring(nextIndex).TrimEnd() + ' ');
        }
    }
}
