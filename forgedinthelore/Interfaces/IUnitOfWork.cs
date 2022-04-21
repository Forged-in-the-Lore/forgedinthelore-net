namespace forgedinthelore_net.Interfaces;

public interface IUnitOfWork
{
    // IRepository Repository { get; }

    Task<bool> Complete();
    bool HasChanges();
}