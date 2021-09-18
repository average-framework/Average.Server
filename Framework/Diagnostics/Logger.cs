using SDK.Shared.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Average.Server.Framework.Diagnostics
{
    public static class Logger
    {
        public class TextColor
        {
            public ConsoleColor Background { get; }
            public ConsoleColor Foreground { get; }

            public TextColor(ConsoleColor background, ConsoleColor foreground)
            {
                Background = background;
                Foreground = foreground;
            }
        }

        public static bool IsDebug { get; set; }

        private static List<ILogInfo> _logs = new List<ILogInfo>();

        public static void Debug(string message)
        {
            if (IsDebug)
            {
                InternalLog(message, LogLevel.Debug);
            }
        }

        public static void Error(string message)
        {
            InternalLog(message, LogLevel.Error);
        }

        public static void Error(Exception exception)
        {
            InternalLog(exception.StackTrace, LogLevel.Error);
        }

        public static void Error(string message, Exception exception)
        {
            InternalLog($"{message}: {exception.Message}", LogLevel.Error);
        }

        public static void Error(string message, LogLevel logLevel)
        {
            InternalLog(message, logLevel);
        }

        public static void Info(string message)
        {
            InternalLog(message, LogLevel.Info);
        }

        public static void Trace(string message)
        {
            InternalLog(message, LogLevel.Trace);
        }

        public static void Warn(string message)
        {
            InternalLog(message, LogLevel.Warn);
        }

        private static void InternalLog(string message, LogLevel level)
        {
            if (_logs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(GetSeparator());
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" [{DateTime.Now.ToLocalTime().ToString("HH:mm:ss")}] ");

            switch (level)
            {
                case LogLevel.Trace:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(" Trace ");
                    break;
                case LogLevel.Debug:
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" Debug ");
                    break;
                case LogLevel.Info:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" Info ");
                    break;
                case LogLevel.Warn:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" Warn ");
                    break;
                case LogLevel.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" Error ");
                    break;
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($" | {message}\n");
        }

        public static void Write(string label, string message, params TextColor[] colors)
        {
            var colorsArray = new List<TextColor>
            {
                new TextColor(ConsoleColor.Black, ConsoleColor.DarkGray),
                new TextColor(ConsoleColor.DarkYellow, ConsoleColor.White),
                new TextColor(ConsoleColor.Black, ConsoleColor.White),
            };

            colors.ToList().ForEach(x => colorsArray.Add(x));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(GetSeparator());
            Console.Write(" ");
            WriteInternalLog($"%[{DateTime.Now.ToLocalTime().ToString("HH:mm:ss")}]% % {label} % | %%{message}", colorsArray.ToArray());
        }

        internal static void WriteInternalLog(string message, params TextColor[] colors)
        {
            try
            {
                var chars = message.ToCharArray();
                var indexes = new List<int>();

                for (int i = 0; i < chars.Length; i++) if (chars[i] == '%') indexes.Add(i);
                for (int i = 0; i < indexes.Count; i++)
                {
                    Console.ResetColor();

                    if (i % 2 == 0)
                    {
                        var startIndex = indexes[i];
                        var nextIndex = indexes[i + 1];
                        var count = nextIndex - startIndex + 1;
                        var sentence = string.Join("", chars.ToList().GetRange(startIndex, count));

                        if (colors.Length > 0)
                        {
                            try
                            {
                                var colorIndex = i / 2;
                                var color = colors[colorIndex];
                                Console.BackgroundColor = color.Background;
                                Console.ForegroundColor = color.Foreground;
                            }
                            catch
                            {
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        Console.Write(sentence.Replace("%", ""));
                    }
                    else
                    {
                        if (i < indexes.Count - 1)
                        {
                            var startIndex = indexes[i] + 1;
                            var nextIndex = indexes[i + 1];
                            var count = nextIndex - startIndex;
                            var sentence = string.Join("", chars.ToList().GetRange(startIndex, count));
                            Console.Write(sentence);
                        }
                        else
                        {
                            var startIndex = indexes[indexes.Count - 1] + 1;
                            var sentence = string.Join("", chars.ToList().GetRange(0, chars.Length)).Substring(startIndex);
                            Console.WriteLine(sentence);
                        }
                    }
                }

                Console.ResetColor();
            }
            catch
            {

            }
        }

        public static void Clear() => Console.Clear();

        public static IEnumerable<ILogInfo> GetLogs(LogLevel level) => _logs.Where(x => x.Level == level);

        private static string GetSeparator()
        {
            var result = "";

            for (int i = 0; i < Console.WindowWidth - 1; i++)
                result += "-";

            return result;
        }
    }
}
