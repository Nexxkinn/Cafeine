using System.Threading.Tasks;

namespace Cafeine.Model {
    public interface IDataService {
        Task<DataItem> GetData();
    }
}