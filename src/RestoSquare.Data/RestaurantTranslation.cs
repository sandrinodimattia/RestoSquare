using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class RestaurantTranslation 
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

        public string Language
        {
            get;
            set;
        }
        
        public string Description
        {
            get;
            set;
        }
        public string Setting
        {
            get;
            set;
        }


        public string Closing
        {
            get;
            set;
        }

        public string Holidays
        {
            get;
            set;
        }
    }
}