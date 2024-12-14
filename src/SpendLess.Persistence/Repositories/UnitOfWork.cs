using Microsoft.AspNetCore.Http;
using SpendLess.Application.Constants;
using SpendLess.Application.Contracts.Persistence;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpendLess.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly SpendLessDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UnitOfWork(SpendLessDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        //public ILeaveAllocationRepository LeaveAllocationRepository => 
        //    _leaveAllocationRepository ??= new LeaveAllocationRepository(_context);
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
        }
    }
}
