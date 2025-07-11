﻿using Pharma_LinkAPI.DTO.MdeicineDTO;
using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface ImedicineRepositiry : Irepo<Medicine>
    {
        Task<IDictionary<int, MedicineViewDTO>> GetMedicinesForCompany(int companyId);
        Task<IDictionary<int, Medicine>> GetMedicinesForCompanyTracking(int companyId);
        
        Task<IEnumerable<Medicine?>> GetMedicinesWithPages(int pageNumber, int pageSize);

        Task<int> sz();

        Task<Medicine?> GetMedicineCompany(int companyId);
        Task<(string Name, string UserName)?> GetCompanyDetails(int id);

        Task<bool> IsUExist(int companyId, string name);
    }
}
