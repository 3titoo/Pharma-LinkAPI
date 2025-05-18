using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface ImedicineRepositiry : Irepo<Medicine>
    {
        Task<IEnumerable<Medicine>> Search(string word);
        Task<IDictionary<int, Medicine>> GetMedicinesForCompany(int companyId);

        Task<Medicine?> GetMedicineCompany(int companyId);
    }
}
