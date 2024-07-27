internal unsafe class Program
{
    /// <summary>
    /// run---sc create {AppDomain.CurrentDomain.FriendlyName} binpath= "{exepath}"---to create this service!
    /// </summary>

    private static void Main(string[] args)
    {
        ServiceBase[] ServicesToRun;
        ServicesToRun =
        [
            new Service1()
        ];
        ServiceBase.Run(ServicesToRun.First());
    }
}
