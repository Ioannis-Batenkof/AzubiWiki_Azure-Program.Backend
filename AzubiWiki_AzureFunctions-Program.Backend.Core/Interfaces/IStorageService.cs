using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces
{
    public interface IStorageService<T>
    {
        public Task Create(T item);
        public Task<List<T>> ReadAll();
        public Task<T> Read(Guid ID);
        public Task Update(T item);
        public Task DeleteAll();
        public Task Delete(Guid ID);
    }
}
