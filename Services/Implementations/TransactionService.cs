using Microsoft.EntityFrameworkCore;
using ParkNet.Data;
using ParkNet.Entities.Operations;
using ParkNet.Entities.Enums;


public class TransactionService : ITransactionService
{
    private readonly ParkNetDbContext _context;

    public TransactionService(ParkNetDbContext context)
    {
        _context = context;
    }

    public async Task<ParkingTransaction> StartParkingAsync(string userId, int slotId)
    {
        var transaction = new ParkingTransaction
        {
            UserId = userId,
            ParkingSlotId = slotId,
            EntryTime = DateTime.Now
        };

        var slot = await _context.ParkingSlots.FindAsync(slotId);
        if (slot != null)
        {
            slot.Status = SlotStatus.Ocupado;
        }

        await _context.ParkingTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<decimal> EndParkingAsync(int transactionId)
    {
        var transaction = await _context.ParkingTransactions
            .Include(t => t.User)
            .Include(t => t.ParkingSlot)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null || transaction.ExitTime != null)
            throw new Exception("Transação inválida");

        transaction.ExitTime = DateTime.Now;
        var duration = (transaction.ExitTime.Value - transaction.EntryTime).TotalMinutes;

        decimal ratePerMinute = 0.10M;
        decimal amount = (decimal)duration * ratePerMinute;

        transaction.AmountCharged = amount;
        transaction.Paid = true;
       
        if (transaction.User != null)
        {
            transaction.User.Balance -= amount;
        }

        if (transaction.ParkingSlot != null)
        {
            transaction.ParkingSlot.Status = SlotStatus.Livre;
        }

        await _context.SaveChangesAsync();
        return amount;
    }
}
