using SpendLess.Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Application.Contracts.Persistence.Token
{
    public interface ITokenRepository
    {
        Task Save(ApplicationTokenBase token);
    }
}
