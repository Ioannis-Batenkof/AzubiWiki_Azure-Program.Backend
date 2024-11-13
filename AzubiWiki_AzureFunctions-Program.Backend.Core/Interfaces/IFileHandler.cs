using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces
{
    public interface IFileHandler
    {
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string content);
        bool Exists(string path);
        void Delete(string path);
        FileStream Create(string path);

    }
}
