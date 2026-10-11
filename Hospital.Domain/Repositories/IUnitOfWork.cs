namespace Hospital.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}