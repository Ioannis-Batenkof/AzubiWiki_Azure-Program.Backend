
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions
{
    public class NoBackupsFoundException : Exception
    {
        public NoBackupsFoundException(string database) : base($"No backup blob found for the {database} database!") { }
    }
}
