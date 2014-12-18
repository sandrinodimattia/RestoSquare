namespace RestoSquare.Domain
{
    public class RegisterRestoCommand
    {

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

        public int[] SelectedAccommodationIds
        {
            get;
            set;
        }
    }
}
