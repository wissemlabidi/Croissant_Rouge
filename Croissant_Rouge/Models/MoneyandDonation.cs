using System;
namespace Croissant_Rouge.Models
{
    public class MoneyandDonation
    {
        public List<Donation> Alldonations { get; set; } = new();
        public List<Money> AllMoney { get; set; } = new();
    }
}



