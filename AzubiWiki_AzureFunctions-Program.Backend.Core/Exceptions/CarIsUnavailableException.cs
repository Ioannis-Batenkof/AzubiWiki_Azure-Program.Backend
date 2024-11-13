using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions
{
    public class CarIsUnavailableException : Exception
    {
        public CarIsUnavailableException() : base("This car is already assigned to another garage.") { }
    }
}
