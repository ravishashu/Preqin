using System.ComponentModel.DataAnnotations;

namespace Preqin.Domain
{
    public class Investor
    {
        [Key]
        public int Id { get; set; }
        public string InvestorName { get; set; }
        public string InvestorType { get; set; }
        public string InvestorCountry { get; set; }
        public DateTime InvestorDateAdded { get; set; }
        public DateTime InvestorLastUpdated { get; set; }
        public List<Commitment> Commitments { get; set; } = new List<Commitment>();
    }
}
