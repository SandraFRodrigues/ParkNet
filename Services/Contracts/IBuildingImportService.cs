using ParkNet.Entities.Infrastructure;
using System.Threading.Tasks;

namespace ParkNet.Services.Contracts
{
    public interface IBuildingImportService
    {
        Task<bool> ImportFromTextAsync(string importedText, string buildingName);
    }
}