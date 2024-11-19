using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Services
{
    public class FileHandler : IFileHandler
    {

        public async Task<string> ReadAllTextAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteAllTextAsync(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            File.WriteAllTextAsync(path, "");
        }

        public FileStream Create(string path)
        {
             return File.Create(path); 
        }
    }
}
