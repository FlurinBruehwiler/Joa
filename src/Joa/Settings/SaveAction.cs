namespace Joa.Settings;

public record SaveAction(Action<object>? Callback, Func<object, Task>? AsyncCallback, Type Type, string Property, bool IsAsync);