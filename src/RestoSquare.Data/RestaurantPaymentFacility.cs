using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class RestaurantPaymentFacility
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant
        {
            get;
            set;
        }

        public Guid RestaurantId
        {
            get;
            set;
        }

        [ForeignKey("PaymentFacilityId")]
        public PaymentFacility PaymentFacility
        {
            get;
            set;
        }

        public int PaymentFacilityId
        {
            get;
            set;
        }
    }
}