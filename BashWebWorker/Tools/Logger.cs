namespace BashWebWorker.Tools
{
    public static class Logger
    {
        private static string DateNowFormat() => DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");

        public static void Debug(string message) => Console.WriteLine($"{DateNowFormat()} DEBUG {message}");
        public static void Error(string message) => Console.WriteLine($"{DateNowFormat()} ERROR {message}");
    }
}
