using System.Runtime.InteropServices;

public unsafe class ServiceBase
{
    public IntPtr ServiceStatus = nint.Zero;
    public string ServiceName => AppDomain.CurrentDomain.FriendlyName;

    public enum ServiceState : uint
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
    public enum ControlsAccepted : uint
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
    public enum ServiceTypes : uint
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

    public delegate void LPSERVICE_MAIN_FUNCTIONA(
        uint dwNumServicesArgs,
        [MarshalAs(UnmanagedType.LPStr)]
        string lpServiceArgVectors);

    public static void Run(ServiceBase service)
    {
        service.Initialize();
    }

    private void Initialize()
    {
        SERVICE_TABLE_ENTRYA entry = new SERVICE_TABLE_ENTRYA()
        {
            lpServiceName = Marshal.StringToHGlobalAnsi(AppDomain.CurrentDomain.FriendlyName),
            lpServiceProc = Marshal.GetFunctionPointerForDelegate(new LPSERVICE_MAIN_FUNCTIONA(ServiceProc))
        };
        if (!StartServiceCtrlDispatcherA(&entry)) Environment.Exit(-1);
    }

    public virtual void OnStart() { }

    public virtual void OnStop() { }

    public bool Stop()
    {
        return SetServiceStatus(ServiceStatus, ServiceState.SERVICE_STOPPED, 0);
    }

    public delegate void LPHANDLER_FUNCTION(ServiceState dwControl);

    private void ServiceProc(uint dwNumServicesArgs, string lpServiceArgVectors)
    {
        ServiceStatus = RegisterServiceCtrlHandlerA(ServiceName, Marshal.GetFunctionPointerForDelegate(new LPHANDLER_FUNCTION(HandlerProc)));
        if (ServiceStatus == nint.Zero) Environment.Exit(-1);
        SetServiceStatus(ServiceStatus, ServiceState.SERVICE_RUNNING, ControlsAccepted.SERVICE_ACCEPT_STOP);
        OnStart();
    }

    public bool SetServiceStatus(IntPtr handle, ServiceState status, ControlsAccepted accept)
    {
        SERVICE_STATUS ss = new SERVICE_STATUS()
        {
            dwServiceType = ServiceTypes.SERVICE_WIN32_OWN_PROCESS,
            dwCurrentState = status,
            dwControlsAccepted = accept
        };
        return SetServiceStatus(handle, &ss);
    }

    private void HandlerProc(ServiceState dwControl)
    {
        switch (dwControl)
        {
            case ServiceState.SERVICE_STOPPED:
                OnStop();
                SetServiceStatus(ServiceStatus, ServiceState.SERVICE_STOPPED, 0);
                break;
        }
    }

    [DllImport("Advapi32.dll")]
    private static extern bool StartServiceCtrlDispatcherA(SERVICE_TABLE_ENTRYA* lpServiceStartTable);

    [DllImport("Advapi32.dll")]
    private static extern bool SetServiceStatus(IntPtr hServiceStatus, SERVICE_STATUS* lpServiceStatus);

    [DllImport("Advapi32.dll")]
    private static extern nint RegisterServiceCtrlHandlerA(
        [MarshalAs(UnmanagedType.LPStr)]
        string lpServiceName,
        IntPtr lpHandlerProc);

    [StructLayout(LayoutKind.Sequential)]
    private struct SERVICE_TABLE_ENTRYA
    {
        public IntPtr lpServiceName;
        public IntPtr lpServiceProc;
        public fixed byte Dummy[16];
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SERVICE_STATUS
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