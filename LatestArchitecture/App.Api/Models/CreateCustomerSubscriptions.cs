namespace App.Api.Models
{
    public class CreateCustomerSubscriptions
    {
        public int OrganizationId { get; set; }

        public int RequestId { get; set; }

        public int AgencySubscriptionDetailID { get; set; }

        public decimal Amount { get; set; }
        public decimal RefundAmount { get; set; }
    }
    
}
