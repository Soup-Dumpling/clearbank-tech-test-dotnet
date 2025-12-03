using System;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors) 
        {
            Errors = errors;
        }
    }
}
