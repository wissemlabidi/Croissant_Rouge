using System;
namespace Croissant_Rouge.Models
{
	public class ShipmentAndDonations
	{
		public List<Donation> Alldonations { get; set; } = new();
		public List<Shipment> AllShippments { get; set; } = new ();
	}
}


