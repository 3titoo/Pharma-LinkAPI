using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.DTO
{
    public class GetAllMedicinesDTO
    {
        [Range(1,int.MaxValue)]
        public int pageNumber { get; set; } = 1;
        [Range(1, int.MaxValue)]
        public decimal pageSize { get; set; } = 10;
    }
}
