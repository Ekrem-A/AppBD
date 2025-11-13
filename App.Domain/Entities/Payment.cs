using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Enums;

namespace App.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
    }
}
