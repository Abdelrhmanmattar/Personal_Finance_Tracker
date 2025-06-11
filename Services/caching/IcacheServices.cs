using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.caching
{
    public interface IcacheServices
    {
        Task SetAsync<T>(string key , T value) where T:class;
        Task<T?> GetAsync<T>(string key) where T:class;
        Task Remove(string key);
        Task<IEnumerable<T>> GetAllfromCache<T>(string ID) where T : class;
        void removeAllKeys();
    }
}
