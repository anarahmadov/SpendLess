using SpendLess.Application.Contracts.Persistence.Token;
using System;
using System.Threading.Tasks;

namespace SpendLess.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ITokenRepository TokenRepository { get; }
        Task Save();
    }
}
