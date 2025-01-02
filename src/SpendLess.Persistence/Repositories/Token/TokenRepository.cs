using SpendLess.Application.Contracts.Persistence.Token;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity;
using SpendLess.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Persistence.Repositories.Token
{
    public class TokenRepository : GenericRepository<ApplicationToken, SpendLessIdentityDbContext>, ITokenRepository
    {
        private readonly SpendLessIdentityDbContext _context;
        public TokenRepository(SpendLessIdentityDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Save(ApplicationTokenBase token)
        {
            var refreshToken = new ApplicationToken()
            {
                TokenString = token.TokenString,
                UserId = token.UserId,
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            await _context.ApplicationTokens.AddAsync(refreshToken);
        }
    }
}
