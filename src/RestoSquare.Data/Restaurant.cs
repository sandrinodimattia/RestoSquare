using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class Restaurant
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        public int? RegionId
        {
            get;
            set;
        }

        [ForeignKey("RegionId")]
        public Region Region
        {
            get;
            set;
        }

        public string InternalName
        {
            get;
            set;
        }


        public string Name
        {
            get;
            set;
        }

        public string PostalCode
        {
            get;
            set;
        }

        public string Locality
        {
            get;
            set;
        }

        public string StreetAddress
        {
            get;
            set;
        }

        public string Website
        {
            get;
            set;
        }

        public int? Budget
        {
            get;
            set;
        }
        
        public double? Latitude
        {
            get;
            set;
        }

        public double? Longitude
        {
            get;
            set;
        }

        public int? Rating
        {
            get;
            set;
        }

        public string Fax
        {
            get;
            set;
        }

        public string Mobile
        {
            get;
            set;
        }
        public string PhoneNumber
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }
        public bool HasImage
        {
            get;
            set;
        }
        
        public ICollection<RestaurantTranslation> Translations
        {
            get;
            set;
        }

        public ICollection<RestaurantAccommodation> Accommodations
        {
            get;
            set;
        }
        public ICollection<RestaurantPaymentFacility> PaymentFacilities
        {
            get;
            set;
        }

        public ICollection<RestaurantCuisine> Cuisines
        {
            get;
            set;
        }
        public Restaurant()
        {
            Accommodations = new Collection<RestaurantAccommodation>();
            PaymentFacilities = new Collection<RestaurantPaymentFacility>();
            Cuisines = new Collection<RestaurantCuisine>();
            Translations = new Collection<RestaurantTranslation>();
        }
    }
}
