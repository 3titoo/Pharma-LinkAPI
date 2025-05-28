using Microsoft.AspNetCore.Mvc;
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

        public async Task Add(Request entity)
        {
            await _db.Requests.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var request = await _db.Requests.FirstOrDefaultAsync(i => i.Id == id);
            if (request != null)
            {
                _db.Requests.Remove(request);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Request?>> GetAll()
        {
            var requests = await _db.Requests.ToListAsync();
            foreach (var request in requests)
            {
                if(request.CreatedAt.AddDays(1) < DateTime.UtcNow && request.IsEmailConfirmed == false)
                {
                    await Delete(request.Id);
                }
            }
            var ret = await _db.Requests.AsNoTracking().Where(r => r.IsEmailConfirmed == true).ToListAsync();
            return ret;
        }

        public async Task<Request?> GetById(int id)
        {
            var request = await _db.Requests.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            return request;
        }

        public async Task<Request?> GetUserByEmail(string email)
        {
            return await _db.Requests.FirstOrDefaultAsync(i => i.Email == email);
        }

        public async Task<Request?> GetUserByusername(string username)
        {
            return await _db.Requests.AsNoTracking().FirstOrDefaultAsync(i => i.UserName == username);
        }

        public async Task Update(Request entity)
        {
            _db.Requests.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
