using System.Collections.Generic;

using System.Web.Mvc;
using RestoSquare.Data;

namespace RestoSquare.Admin.Models
{
    public class RegisterViewModel
    {
        public IEnumerable<SelectListItem> Regions
        {
            get;
            set;
        }

        public IEnumerable<SelectListItem> Cuisines
        {
            get;
            set;
        }

        public IEnumerable<AccommodationTranslation> Accomodations
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Region
        {
            get;
            set;
        }

        public string Cuisine
        {
            get;
            set;
        }

        public int Budget
        {
            get;
            set;
        }

        public int Rating
        {
            get;
            set;
        }

        public string City
        {
            get;
            set;
        }

        public string Street
        {
            get;
            set;
        }

        public AccommodationTranslation[] SelectedAccommodations
        {
            get;
            set;
        }

        public int[] SelectedAccommodationIds
        {
            get;
            set;
        }
    }
}