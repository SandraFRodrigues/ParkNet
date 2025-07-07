using ParkNet.Entities.Enums;
using ParkNet.Entities.Operations;


namespace ParkNet.Services.Contracts
{
    public interface ISubscriptionService
    {
        Task<bool> CreateSubscriptionAsync(string userId, int parkingSlotId, SubscriptionType type);
    }
}
