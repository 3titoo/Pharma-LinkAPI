using Pharma_LinkAPI.Services.EmailService;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface IUnitOfWork
    {
        ICartRepositry _cartRepositry { get; }
        IAccountRepositry _accountRepositry { get; }
        IOrderRepositry _orderRepositry { get; }
        IrequestRepositry _requestRepositry { get; }
        IreviewRepositiry _reviewRepositiry { get; }
        ImedicineRepositiry _medicineRepositiry { get; }
        IEmailService _emailService { get; }
        Task SaveAsync();
        Task BeginTransactionAsync();
        Task RollbackAsync();
        Task CommitAsync();
    }
}
