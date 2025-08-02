namespace DemoApp.Domain.Common;

public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; }

    protected AggregateRoot(TId id)
    {
        Id = id;
    }
}
