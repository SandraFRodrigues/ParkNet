using ParkNet.Entities.Operations;

public interface ITransactionService
{
    Task<ParkingTransaction> StartParkingAsync(string userId, int slotId);
    Task<decimal> EndParkingAsync(int transactionId);
}
