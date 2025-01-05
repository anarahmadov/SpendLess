using Microsoft.EntityFrameworkCore;
using SpendLess.Application.Contracts.Persistence.Token;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity;
using SpendLess.Identity.Models;

namespace SpendLess.Persistence.Repositories.Token
{
    public class TokenRepository : GenericRepository<ApplicationToken, SpendLessIdentityDbContext>, ITokenRepository
    {
        private readonly SpendLessIdentityDbContext _context;
        public TokenRepository(SpendLessIdentityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationTokenBase> GetToken(string token, bool isRevoked = false)
        {
            return await _context.ApplicationTokens.FirstOrDefaultAsync(x => x.TokenString == token && x.IsRevoked == isRevoked);
        }

        public async Task<IQueryable<ApplicationTokenBase>> GetTokensByUserId(int userId, bool isRevoked = false)
        {
             return _context.ApplicationTokens
                .Where(x => x.UserId == userId && x.IsRevoked == isRevoked);
        }

        public async Task RevokeToken(string token)
        {
            var applicationToken = await GetToken(token);
            applicationToken.IsRevoked = true;
        }

        public async Task Save(ApplicationTokenBase token)
        {
            var applicationToken = (ApplicationToken)token;
            await _context.ApplicationTokens.AddAsync(applicationToken);
        }

        
    }
}
