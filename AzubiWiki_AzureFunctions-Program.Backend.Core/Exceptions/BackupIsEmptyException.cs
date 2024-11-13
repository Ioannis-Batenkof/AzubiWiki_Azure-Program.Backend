using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions
{
    public class BackupIsEmptyException : Exception
    {
        public BackupIsEmptyException(string blob) : base($"The blob {blob} returned an empty backup.") { }
    }
}
