using ParkNet.Services.Contracts;
using System.Threading.Tasks;

namespace ParkNet.Services.Contracts
{
    public interface IImportService
    {
        Task<bool> CreateBuildingFromLayoutAsync(string buildingName, string layout);
    }
}
