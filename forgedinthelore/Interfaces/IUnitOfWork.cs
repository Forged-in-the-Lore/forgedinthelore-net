namespace forgedinthelore_net.Interfaces;

public interface IUnitOfWork
{

    Task<bool> Complete();
    bool HasChanges();
}