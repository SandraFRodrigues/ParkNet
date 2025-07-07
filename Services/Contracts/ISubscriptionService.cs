using ParkNet.Entities.Operations;
using ParkNet.Entities.Operations.Enums;


namespace ParkNet.Services.Contracts
{
    public interface ISubscriptionService
    {
        Task<bool> CreateSubscriptionAsync(string userId, int parkingSlotId, SubscriptionType type);
    }
}
