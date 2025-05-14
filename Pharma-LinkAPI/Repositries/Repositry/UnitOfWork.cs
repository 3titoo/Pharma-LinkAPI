using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public ICartRepositry _cartRepositry { get; }
        public IAccountRepositry _accountRepositry { get; }
        public IOrderRepositry _orderRepositry { get; }
        public IrequestRepositry _requestRepositry { get; }
        public IreviewRepositiry _reviewRepositiry { get; }
        public ImedicineRepositiry _medicineRepositiry { get; }
        public UnitOfWork(ICartRepositry cartRepositry, IAccountRepositry accountRepositry, IOrderRepositry orderRepositry, IrequestRepositry requestRepositry, IreviewRepositiry reviewRepositiry, ImedicineRepositiry medicineRepositiry, AppDbContext db)
        {
            _cartRepositry = cartRepositry;
            _accountRepositry = accountRepositry;
            _orderRepositry = orderRepositry;
            _requestRepositry = requestRepositry;
            _reviewRepositiry = reviewRepositiry;
            _medicineRepositiry = medicineRepositiry;
            _db = db;
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
