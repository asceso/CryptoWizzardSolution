using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Services.MemoryService
{
    public class MemoryServiceImplementation : IMemoryService
    {
        private static MemoryCache cache;
        public MemoryServiceImplementation() => cache = MemoryCache.Default;
        public Task StoreItem<TData>(string alias, TData item)
        {
            CacheItemPolicy policy = new();
            policy.SlidingExpiration = TimeSpan.FromMinutes(0);
            cache.Set(alias, item, policy);
            return Task.CompletedTask;
        }
        public Task RemoveItem(string alias)
        {
            if (cache.Contains(alias))
            {
                cache.Remove(alias);
            }
            return Task.CompletedTask;
        }
        public Task<TData> GetItem<TData>(string alias)
        {
            var data = cache.Get(alias);
            if (data is TData item)
            {
                return Task.FromResult(item);
            }
            return Task.FromResult<TData>(default);
        }
    }
}
