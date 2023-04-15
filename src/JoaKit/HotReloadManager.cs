using Modern.WindowKit;

namespace JoaKit;

public static class HotReloadManager
{
    public static void ClearCache(Type[]? updatedTypes)
    {
        Console.WriteLine("HotReloadManager.ClearCache");
    }

    public static void UpdateApplication(Type[]? updatedTypes)
    {
        foreach (var manager in JoaKitApp.Managers)
        {
            manager._renderer.Build(manager.RootComponent);
            manager.DoPaint(new Rect());
        }
        
        Console.WriteLine("HotReloadManager.UpdateApplication");
    }
}
