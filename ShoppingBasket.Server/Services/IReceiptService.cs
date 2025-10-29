using ShoppingBasket.Server.DataTransfer;

namespace ShoppingBasket.Server.Services
{
    public interface IReceiptService
    {
        Task<IEnumerable<ReceiptDto>> GetAllReceiptsAsync();
        Task<ReceiptDto> GetReceiptByIdAsync(long id);
        Task<ReceiptDto> GetDetailedReceiptByIdAsync(long id);
        Task<IEnumerable<ReceiptShortDto>> GetReceiptsHistoryAsync();
        Task<ReceiptDto> CreateReceiptAsync(ReceiptCreateDto receiptCreateDto);
    }
}
