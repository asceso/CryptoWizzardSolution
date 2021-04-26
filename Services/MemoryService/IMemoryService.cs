using System.Threading.Tasks;

namespace Services.MemoryService
{
    public interface IMemoryService
    {
        Task StoreItem<TData>(string alias, TData item);
        Task RemoveItem(string alias);
        Task<TData> GetItem<TData>(string alias);
    }
}
