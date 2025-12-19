namespace App.Common.Models
{
    public class SessionMetadataModel
    {
        public int Price { get; set; }
        public int OrganizationId { get; set; }
        public int ClientCount { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
        public int Discount { get; set; }
        public int BillingCycle { get; set; }
        public int Status { get; set; }
        public int? CustomerId { get; set; }
        public int? SubscriptionId { get; set; }
        public int? Frequency { get; set; }


        public static SessionMetadataModel FromDictionary(IDictionary<string, string> metadata)
        {
            return new SessionMetadataModel
            {
                Price = Convert.ToInt32(metadata["Price"]),
                OrganizationId = Convert.ToInt32(metadata["OrganizationId"]),
                ClientCount = Convert.ToInt32(metadata["ClientCount"]),
                PlanId = Convert.ToInt32(metadata["PlanId"]),
                StartDate = DateTime.Parse(metadata["StartDate"]),
                Note = metadata.ContainsKey("Note") ? metadata["Note"] : null,
                Discount = Convert.ToInt32(metadata["Discount"]),
                BillingCycle = Convert.ToInt32(metadata["BillingCycle"]),
                Status = Convert.ToInt32(metadata["Status"])
            };
        }
    }
}
