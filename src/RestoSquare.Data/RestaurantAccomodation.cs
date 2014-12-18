using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class RestaurantAccommodation
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

        [ForeignKey("AccommodationId")]
        public Accommodation Accommodation
        {
            get;
            set;
        }

        public int AccommodationId
        {
            get;
            set;
        }
    }
}