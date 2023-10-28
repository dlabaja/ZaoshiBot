namespace Zaoshi.Exceptions;

public class FatalException : Exception
{
    public FatalException(string msg) : base(msg)
    {
        HandleException(msg);
    }

    public FatalException(Exception e) : base("", e)
    {
        HandleException($"{e.Message}\n{e.InnerException}\n{e.StackTrace}");
    }

    private static void HandleException(string exception)
    {
        File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "crash_logs", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")), exception);
        Environment.Exit(1);
    }
}
