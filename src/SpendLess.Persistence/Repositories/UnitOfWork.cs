using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SpendLess.Application.Constants;
using SpendLess.Application.Contracts.Persistence;
using SpendLess.Application.Contracts.Persistence.Token;
using SpendLess.Identity;
using SpendLess.Persistence.Repositories.Token;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpendLess.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly SpendLessDbContext _context;
        private readonly SpendLessIdentityDbContext _identityDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ITokenRepository _tokenRepository;
        public UnitOfWork(SpendLessDbContext context, 
                          SpendLessIdentityDbContext identityDbContext,
                          IHttpContextAccessor httpContextAccessor,
                          ITokenRepository tokenRepository)
        {
            _context = context;
            _identityDbContext = identityDbContext;
            _httpContextAccessor = httpContextAccessor;
            _tokenRepository = tokenRepository;
        }
        public ITokenRepository TokenRepository =>
            _tokenRepository ??= new TokenRepository(_identityDbContext);
        //public ILeaveTypeRepository LeaveTypeRepository => 
        //    _leaveTypeRepository ??= new LeaveTypeRepository(_context);
        //public ILeaveRequestRepository LeaveRequestRepository => 
        //    _leaveRequestRepository ??= new LeaveRequestRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save() 
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.Uid)?.Value;

            await _context.SaveChangesAsync(username);
            await _identityDbContext.SaveChangesAsync();
        }
    }
}
