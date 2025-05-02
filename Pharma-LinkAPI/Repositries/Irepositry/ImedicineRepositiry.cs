using Pharma_LinkAPI.Models;

namespace Pharma_LinkAPI.Repositries.Irepositry
{
    public interface ImedicineRepositiry : Irepo<Medicine>
    {
        IEnumerable<Medicine> Search(string word);
    }
}
