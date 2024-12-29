using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendLess.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(ValidationResult validationResult)
        {
            Errors = validationResult.ToDictionary();
        }
    }
}
