using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces
{
    public interface IBackupService
    {
        public Task BackupStorage();
        public Task RestoreFromBackup();
        public Task DeleteBackup();
    }
}
