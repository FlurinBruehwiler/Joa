using Modern.WindowKit.Threading;

namespace JoaKit;

public class JoaSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state)
    {
        Dispatcher.UIThread.Post(() => d(state), DispatcherPriority.Send);
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        if (Dispatcher.UIThread.CheckAccess())
            d(state);
        else
            Dispatcher.UIThread.InvokeAsync(() => d(state), DispatcherPriority.Send).GetAwaiter().GetResult();
    }

    public static void Install()
    {
        SetSynchronizationContext(new JoaSynchronizationContext());
    }
}