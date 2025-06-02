using System;
using System.Text;

namespace FileObjectExtractor.Services
{
    public class CliProgressService : IProgressService
    {
        private const int ProgressBarWidth = 50;
        private readonly object _lock = new();
        private double maximum = 100;
        private string message = string.Empty;
        private bool isVisible;

        public void ShowProgress()
        {
            isVisible = true;
            Console.CursorVisible = false;
        }

        public void HideProgress()
        {
            isVisible = false;
            Console.CursorVisible = true;
            Console.WriteLine(); // Move to next line after progress is complete
        }

        public void SetProgress(double progress)
        {
            if (!isVisible) return;

            lock (_lock)
            {
                int filled = (int)Math.Round((progress / maximum) * ProgressBarWidth);
                filled = Math.Min(filled, ProgressBarWidth);

                StringBuilder bar = new();
                bar.Append('[');
                bar.Append('=', filled);
                if (filled < ProgressBarWidth)
                {
                    bar.Append('>');
                    bar.Append(' ', ProgressBarWidth - filled - 1);
                }
                bar.Append(']');

                int percentage = (int)Math.Round((progress / maximum) * 100);

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"\r{bar} {percentage}% {message}".PadRight(Console.WindowWidth - 1));
            }
        }

        public void SetMaximum(double maximum)
        {
            this.maximum = maximum;
        }

        public void SetMessage(string message)
        {
            this.message = message;
        }
    }
}
