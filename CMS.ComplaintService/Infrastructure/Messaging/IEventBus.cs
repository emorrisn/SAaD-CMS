namespace CMS.ComplaintService.Infrastructure.Messaging;

public interface IEventBus
{
    void Publish<T>(T message);
    void Subscribe<T>(Action<T> handler);
}
