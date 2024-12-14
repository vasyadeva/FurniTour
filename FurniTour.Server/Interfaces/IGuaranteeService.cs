using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Guarantee;

namespace FurniTour.Server.Interfaces
{
    public interface IGuaranteeService
    {
        public Task<List<GuaranteeModel>> GetGuarantees();
        public List<GuaranteeModel> GetMyGuarantees();
        public Task<GuaranteeModel> GetGuarantee(int guaranteeId);
        public string AddGuarantee(GuaranteeAddModel guarantee);
        public bool IsGuaranteeValid(int guaranteeId);
        public void UpdateGuarantee(int guaranteeId, string status);

    }
}
