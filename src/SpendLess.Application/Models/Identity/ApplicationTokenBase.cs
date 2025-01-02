using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Application.Models.Identity
{
    public abstract class ApplicationTokenBase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenString { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
