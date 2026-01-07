using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Dto
{
    public class CheckoutSessionRequest
    {
        public int Id { get; set; }
        public string PriceId { get; set; }
        public int? Quantity { get; set; } = 1;
        public int? ExtraClientCount { get; set; } = 0;
        public int? PerClientCost { get; set; }
        public int? PlanId { get; set; }
        public int? OrganizationId { get; set; }
        public int? ClientCount { get; set; }
        public string BusinessName { get; set; }
    }

}