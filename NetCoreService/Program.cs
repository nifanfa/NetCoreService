using System.Runtime.InteropServices;


internal unsafe class Program
{
    static IntPtr ServiceStatus = nint.Zero;

    enum ServiceState : uint
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007
    }

    [Flags]
    enum ControlsAccepted : uint
    {
        SERVICE_ACCEPT_STOP = 0x00000001,
        SERVICE_ACCEPT_PAUSE_CONTINUE = 0x00000002,
        SERVICE_ACCEPT_SHUTDOWN = 0x00000004,
        SERVICE_ACCEPT_PARAMCHANGE = 0x00000008,
        SERVICE_ACCEPT_NETBINDCHANGE = 0x00000010,
        SERVICE_ACCEPT_HARDWAREPROFILECHANGE = 0x00000020,
        SERVICE_ACCEPT_POWEREVENT = 0x00000040,
        SERVICE_ACCEPT_SESSIONCHANGE = 0x00000080,
        SERVICE_ACCEPT_PRESHUTDOWN = 0x00000100,
        SERVICE_ACCEPT_TIMECHANGE = 0x00000200,
        SERVICE_ACCEPT_TRIGGEREVENT = 0x00000400,
        SERVICE_ACCEPT_USER_LOGOFF = 0x00000800,
        SERVICE_ACCEPT_LOWRESOURCES = 0x00002000,
        SERVICE_ACCEPT_SYSTEMLOWRESOURCES = 0x00004000
    }

    [Flags]
    enum ServiceTypes : uint
    {
        SERVICE_KERNEL_DRIVER = 0x00000001,
        SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,
        SERVICE_ADAPTER = 0x00000004,
        SERVICE_RECOGNIZER_DRIVER = 0x00000008,
        SERVICE_WIN32_OWN_PROCESS = 0x00000010,
        SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
        SERVICE_USER_SERVICE = 0x00000040,
        SERVICE_USERSERVICE_INSTANCE = 0x00000080,
        SERVICE_INTERACTIVE_PROCESS = 0x00000100,
        SERVICE_PKG_SERVICE = 0x00000200
    }

    /// <summary>
    /// run---sc create {ServiceName} binpath= "{exepath}"---to create this service!
    /// </summary>
    const string ServiceName = "NetCoreService";

    private static void Main(string[] args)
    {
        SERVICE_TABLE_ENTRYA entry = new SERVICE_TABLE_ENTRYA()
        {
            lpServiceName = Marshal.StringToHGlobalAnsi(ServiceName),
            lpServiceProc = &ServiceProc
        };
        if (!StartServiceCtrlDispatcherA(&entry)) Environment.Exit(-1);
    }

    static void ServiceProc(uint dwNumServicesArgs, nint lpServiceArgVectors)
    {
        ServiceStatus = RegisterServiceCtrlHandlerA(ServiceName, &HandlerProc);
        if (ServiceStatus == nint.Zero) Environment.Exit(-1);
        SetServiceStatus(ServiceStatus, ServiceState.SERVICE_RUNNING, ControlsAccepted.SERVICE_ACCEPT_STOP);

        for (; ; )
        {
            if(MessageBox.Show("Info", "Service started successfully!\r\nDo you want to close this service rightaway?", MessageBoxButtons.OKCancel) == MessageBoxResult.OK)
            {
                SetServiceStatus(ServiceStatus, ServiceState.SERVICE_STOPPED, 0);
                break;
            }
        }
    }

    static bool SetServiceStatus(IntPtr handle, ServiceState status, ControlsAccepted accept)
    {
        SERVICE_STATUS ss = new SERVICE_STATUS()
        {
            dwServiceType = ServiceTypes.SERVICE_WIN32_OWN_PROCESS,
            dwCurrentState = status,
            dwControlsAccepted = accept
        };
        return SetServiceStatus(handle, &ss);
    }

    static void HandlerProc(ServiceState dwControl)
    {
        switch (dwControl)
        {
            case ServiceState.SERVICE_STOPPED:
                SetServiceStatus(ServiceStatus, ServiceState.SERVICE_STOPPED, 0);
                break;
        }
    }

    [DllImport("Advapi32.dll")]
    static extern bool StartServiceCtrlDispatcherA(SERVICE_TABLE_ENTRYA* lpServiceStartTable);

    [DllImport("Advapi32.dll")]
    static extern bool SetServiceStatus(IntPtr hServiceStatus, SERVICE_STATUS* lpServiceStatus);

    [DllImport("Advapi32.dll")]
    static extern nint RegisterServiceCtrlHandlerA(
        [MarshalAs(UnmanagedType.LPStr)]
        string lpServiceName,
        delegate*<ServiceState, void> lpHandlerProc);

    [StructLayout(LayoutKind.Sequential)]
    struct SERVICE_TABLE_ENTRYA
    {
        public IntPtr lpServiceName;
        public delegate*<uint, IntPtr, void> lpServiceProc;
        public fixed byte Dummy[16];
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SERVICE_STATUS
    {
        public ServiceTypes dwServiceType;
        public ServiceState dwCurrentState;
        public ControlsAccepted dwControlsAccepted;
        public uint dwWin32ExitCode;
        public uint dwServiceSpecificExitCode;
        public uint dwCheckPoint;
        public uint dwWaitHint;
    }
}
