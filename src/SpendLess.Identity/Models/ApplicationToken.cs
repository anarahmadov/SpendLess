using SpendLess.Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Models
{
    public class ApplicationToken : ApplicationTokenBase
    {
        public ApplicationUser User { get; set; }
    }
}
