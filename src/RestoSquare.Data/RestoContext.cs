using System.Data.Entity;

namespace RestoSquare.Data
{
    public class RestoContext : DbContext
    {
        public IDbSet<Restaurant> Restaurants
        {
            get;
            set;
        }
        public IDbSet<RestaurantTranslation> RestaurantTranslations
        {
            get;
            set;
        }
        public IDbSet<Accommodation> Accommodations
        {
            get;
            set;
        }
        public IDbSet<AccommodationTranslation> AccommodationTranslations
        {
            get;
            set;
        }
        public IDbSet<Region> Regions
        {
            get;
            set;
        }
        public IDbSet<RegionTranslation> RegionTranslations
        {
            get;
            set;
        }
        public IDbSet<Cuisine> Cuisines
        {
            get;
            set;
        }
        public IDbSet<CuisineTranslation> CuisineTranslations
        {
            get;
            set;
        }
        public IDbSet<PaymentFacility> PaymentFacilities
        {
            get;
            set;
        }
        public IDbSet<PaymentFacilityTranslation> PaymentFacilityTranslations
        {
            get;
            set;
        } 

        public RestoContext(string connectionString = "RestoDb")
            : base(connectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accommodation>();
            modelBuilder.Entity<AccommodationTranslation>();
            modelBuilder.Entity<Cuisine>();
            modelBuilder.Entity<CuisineTranslation>();
            modelBuilder.Entity<PaymentFacility>();
            modelBuilder.Entity<PaymentFacilityTranslation>();
            modelBuilder.Entity<Region>();
            modelBuilder.Entity<RegionTranslation>();
            modelBuilder.Entity<Restaurant>();
            modelBuilder.Entity<RestaurantAccommodation>();
            modelBuilder.Entity<RestaurantCuisine>();
            modelBuilder.Entity<RestaurantPaymentFacility>();
            modelBuilder.Entity<RestaurantTranslation>();
        }
    }
}
