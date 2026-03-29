using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
namespace MaverickLogging
{
    public class MaverickLoggingExtensions
    {

    }

    public sealed class MinimalConsoleFormatter : ConsoleFormatter
    {
        public MinimalConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options)
            : base("minimal")
        {
        }

        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            var timestamp = DateTimeOffset.Now.ToString("HH:mm:ss.fff");
            var level = logEntry.LogLevel.ToString();
            var category = logEntry.Category;
            var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception) ?? "";

            // Skip if empty
            if (string.IsNullOrWhiteSpace(message) && logEntry.Exception == null)
                return;

            var sb = new StringBuilder();

            // Base header: timestamp, level, category
            sb.Append(timestamp.PadRight(12))               // Timestamp column
              .Append(" [").Append(level.PadRight(7)).Append("] ") // Level column
              .Append(category)                             // Full category (no truncation)
              .AppendLine();                                // Newline for message body

            // Indent for message
            var indent = new string(' ', 12 + 10 + 1); // timestamp + level + space

            // Handle long messages with comma-separated items nicely
            if (message.Contains(", "))
            {
                var parts = message.Split(", ");
                sb.AppendLine(indent + parts[0]);
                foreach (var part in parts.Skip(1))
                {
                    sb.AppendLine(indent + "- " + part);
                }
            }
            else
            {
                // Wrap long single-line messages
                var lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    sb.AppendLine(indent + line);
                }
            }

            // Append exception if present
            if (logEntry.Exception != null)
            {
                var exLines = logEntry.Exception.ToString()
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var line in exLines)
                {
                    sb.AppendLine(indent + line);
                }
            }

            textWriter.Write(sb.ToString());
        }
    }
}
