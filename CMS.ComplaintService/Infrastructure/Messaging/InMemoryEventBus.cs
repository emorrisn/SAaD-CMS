namespace CMS.ComplaintService.Infrastructure.Messaging;

public class InMemoryEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<T>(T message)
    {
        if (_handlers.TryGetValue(typeof(T), out var list))
        {
            foreach (var h in list.Cast<Action<T>>())
            {
                try { h(message); } catch { /* swallow for PoC */ }
            }
        }
    }

    public void Subscribe<T>(Action<T> handler)
    {
        if (!_handlers.TryGetValue(typeof(T), out var list))
        {
            list = new List<Delegate>();
            _handlers[typeof(T)] = list;
        }
        list.Add(handler);
    }
}
