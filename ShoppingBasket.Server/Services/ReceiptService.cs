using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.MappingProfiles;
using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private ILoggerFactory _loggerFactory;
        private readonly Mapper _mapper;
        private readonly MapperConfiguration _mapperConfig;

        public ReceiptService(IReceiptRepository receiptRepository, ILoggerFactory loggerFactory)
        {
            _receiptRepository = receiptRepository;
            _loggerFactory = loggerFactory;
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ReceiptMappingProfile());
            }, _loggerFactory);
            _mapper = new Mapper(_mapperConfig);
        }

        public async Task<IEnumerable<ReceiptDto>> GetAllReceiptsAsync()
        {
            var receipts = await _receiptRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Receipt>, IEnumerable<ReceiptDto>>(receipts);
        }

        public async Task<ReceiptDto> GetReceiptByIdAsync(long id)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            return _mapper.Map<Receipt, ReceiptDto>(receipt);
        }

        public async Task<ReceiptDto> GetDetailedReceiptByIdAsync(long id)
        {
            var receipt = await _receiptRepository.GetDetailedByIdAsync(id);
            return _mapper.Map<Receipt, ReceiptDto>(receipt);
        }

        //TODO: refactor this method ?
        public async Task<IEnumerable<ReceiptShortDto>> GetReceiptsHistoryAsync()
        {
            var receipts = await _receiptRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Receipt>, IEnumerable<ReceiptShortDto>>(receipts);
        }

        //TODO: implement CreateReceiptAsync method
        public async Task<ReceiptDto> CreateReceiptAsync(ReceiptCreateDto receiptCreateDto)
        {
            throw new NotImplementedException();
        }
    }
}
