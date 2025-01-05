using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Application.Exceptions
{
    public class UnauthorizedAccessException : ApplicationException
    {
        public UnauthorizedAccessException(string message) : base(message)
        {
            
        }
    }
}
