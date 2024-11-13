using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions
{
    public class UnprocessableNumberValueException : Exception
    {
        public UnprocessableNumberValueException(string variable) : base($"The {variable} variable is of incorrect format!  Please enter a valid number.") { }
    }
}
