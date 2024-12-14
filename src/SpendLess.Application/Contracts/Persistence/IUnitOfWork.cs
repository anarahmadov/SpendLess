using System;
using System.Threading.Tasks;

namespace SpendLess.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task Save();
    }
}
