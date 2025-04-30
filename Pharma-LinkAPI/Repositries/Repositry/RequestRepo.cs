using Microsoft.EntityFrameworkCore;
using Pharma_LinkAPI.Data;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Repositries.Repositry
{
    public class RequestRepo : IrequestRepositry
    {
        private readonly AppDbContext _db;

        public RequestRepo(AppDbContext db)
        {
            _db = db;
        }

        public void Add(Request entity)
        {
            _db.Requests.Add(entity);
            _db.SaveChanges();
        }

        public async void Delete(int id)
        {
            var request = await _db.Requests.FirstAsync(i=> i.Id == id);
            if (request != null)
            {
                _db.Requests.Remove(request);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Request?>> GetAll()
        {
            var requests = await _db.Requests.ToListAsync();
            return requests;
        }

        public async Task<Request?> GetById(int id)
        {
            var request = await _db.Requests.FirstOrDefaultAsync(i => i.Id == id);
            return request;
        }

        public void Update(Request entity)
        {
            throw new NotImplementedException();
        }
    }
}
