using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class RestaurantCuisine
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

        [ForeignKey("CuisineId")]
        public Cuisine Cuisine
        {
            get;
            set;
        }

        public int CuisineId
        {
            get;
            set;
        }
    }
}