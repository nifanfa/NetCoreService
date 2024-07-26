using System.Runtime.InteropServices;

public class MessageBox
{
    public static MessageBoxResult Show(string title, string text, MessageBoxButtons messageBoxButtons)
    {
        var result = WTSSendMessage(nint.Zero, WTSGetActiveConsoleSessionId(), title, title.Length, text, text.Length, (int)messageBoxButtons, 0, out var resp, true);
        return (MessageBoxResult)resp;
    }

    [DllImport("kernel32.dll")]
    static extern int WTSGetActiveConsoleSessionId();

    [DllImport("wtsapi32.dll", SetLastError = true)]
    static extern bool WTSSendMessage(IntPtr hServer,
        int SessionId,
        [MarshalAs(UnmanagedType.LPStr)]
        string pTitle,
        int TitleLength,
        [MarshalAs(UnmanagedType.LPStr)]
        string pMessage,
        int MessageLength,
        int Style,
        int Timeout,
        out int pResponse,
        bool bWait);
}

public enum MessageBoxResult
{
    None,
    OK,
    Cancel,
    Abort,
    Retry,
    Ignore,
    Yes,
    No
}

public enum MessageBoxButtons
{
    OK,
    OKCancel,
    AbortRetryIgnore,
    YesNoCancel,
    YesNo,
    RetryCancel
}