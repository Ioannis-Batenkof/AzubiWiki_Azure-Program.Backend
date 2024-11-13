using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions
{
    public class UnprocessableGuidValueException : Exception
    {
        public UnprocessableGuidValueException(string variable) : base($"The {variable} field was of incorrect format. Please enter a valid ID") { }
    }
}
