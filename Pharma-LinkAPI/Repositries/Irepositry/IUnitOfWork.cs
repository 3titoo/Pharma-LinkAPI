using Pharma_LinkAPI.Data;

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
        Task SaveAsync();
    }
}
