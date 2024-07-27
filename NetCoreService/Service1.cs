public class Service1 : ServiceBase
{
    public Service1()
    {
    }

    public override void OnStart()
    {
        for (; ; )
        {
            if (MessageBox.Show("Info", "Service started successfully!\r\nDo you want to close this service rightaway?", MessageBoxButtons.OKCancel) == MessageBoxResult.OK)
            {
                Stop();
                break;
            }
        }
    }

    public override void OnStop()
    {
    }
}