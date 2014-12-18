using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using RestoSquare.Core.Helpers;
using RestoSquare.Data;
using RestoSquare.Domain;

namespace RestoSquare.Jobs.Realtime
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var host = new JobHost();
            host.RunAndBlock();
        }

        public static void ProcessQueue([QueueTrigger("commands")] RegisterRestoCommand command)
        {
            Console.WriteLine("Processing registration: {0}", command.Name);

            var coordinates = Geocoding.Execute("Belgium", command.City, command.Street);
            if (coordinates == null)
            {
                Console.WriteLine("Unable to find coordinates");
            }
            else
            {
                Console.WriteLine("Coordinates: {0} {1}", coordinates.Latitude, coordinates.Longitude);
            }

            // Persist to database.
            var id = PersistToDatabase(command, coordinates);
            PersistToSearch(id, command, coordinates);
        }

        private static void PersistToSearch(Guid id, RegisterRestoCommand command, Coordinates coordinates)
        {
            using (var ctx = new RestoContext())
            {
                var connection = ApiConnection.Create(CloudConfigurationManager.GetSetting("Azure.Search.ServiceName"),
                    CloudConfigurationManager.GetSetting("Azure.Search.ApiKey"));
                var indexClient = new IndexManagementClient(connection);
                var indexName = CloudConfigurationManager.GetSetting("Azure.Search.IndexName");

                var restaurant = ctx.Restaurants
                    .Include(r => r.Accommodations.Select(a => a.Accommodation.Translations))
                    .FirstOrDefault(r => r.Id == id);


                var operation = new IndexOperation(IndexOperationType.MergeOrUpload, "id", id.ToString())
                    .WithProperty("name", command.Name)
                    .WithProperty("locality", command.City)
                    .WithProperty("budget", command.Budget)
                    .WithProperty("rating", command.Rating)
                    .WithProperty("street", command.Street)
                    .WithProperty("accommodations", restaurant.Accommodations.Select(a => a.Accommodation).TryGet<Accommodation, AccommodationTranslation>("en"))
                    .WithProperty("accommodations_fr", restaurant.Accommodations.Select(a => a.Accommodation).TryGet<Accommodation, AccommodationTranslation>("fr"))
                    .WithProperty("accommodations_nl", restaurant.Accommodations.Select(a => a.Accommodation).TryGet<Accommodation, AccommodationTranslation>("nl"))
                    .WithProperty("cuisine", restaurant.Cuisines.Select(a => a.Cuisine).TryGet<Cuisine, CuisineTranslation>("en"))
                    .WithProperty("cuisine_fr", restaurant.Cuisines.Select(a => a.Cuisine).TryGet<Cuisine, CuisineTranslation>("fr"))
                    .WithProperty("cuisine_nl", restaurant.Cuisines.Select(a => a.Cuisine).TryGet<Cuisine, CuisineTranslation>("nl"));

                if (coordinates != null)
                {
                    operation.WithGeographyPoint("location", coordinates.Longitude, coordinates.Latitude);
                }

                var response = indexClient.PopulateAsync(indexName, operation).Result;

                // Error handling!
                if (!response.IsSuccess)
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    return;
                }
                else
                {
                    var failed = response.Body.Where(r => !r.Status);
                    foreach (var item in failed)
                    {
                        Console.WriteLine("Failed: {0} ({1})", item.Key, item.ErrorMessage);
                    }
                }

                Console.WriteLine("Persisted to Search.");
            }
        }

        private static Guid PersistToDatabase(RegisterRestoCommand command, Coordinates coordinates)
        {
            using (var ctx = new RestoContext())
            {
                var restaurant = ctx.Restaurants.Add(new Restaurant()
                {
                    Id = Guid.NewGuid(),
                    Locality = command.City,
                    StreetAddress = command.Street,
                    Name = command.Name
                });

                if (coordinates != null)
                {
                    restaurant.Latitude = coordinates.Latitude;
                    restaurant.Longitude = coordinates.Longitude;
                }

                if (!String.IsNullOrEmpty(command.Region))
                {
                    restaurant.RegionId = int.Parse(command.Region);
                }

                if (!String.IsNullOrEmpty(command.Cuisine))
                {
                    restaurant.Cuisines = new Collection<RestaurantCuisine>
                    {
                        new RestaurantCuisine
                        {
                            Id = Guid.NewGuid(), 
                            CuisineId = int.Parse(command.Cuisine)
                        }
                    };
                }

                if (command.SelectedAccommodationIds != null)
                {
                    restaurant.Accommodations = new Collection<RestaurantAccommodation>();

                    foreach (var accommodation in command.SelectedAccommodationIds)
                    {
                        restaurant.Accommodations.Add(new RestaurantAccommodation
                        {
                            Id = Guid.NewGuid(),
                            AccommodationId = accommodation
                        });
                    }
                }

                ctx.SaveChanges();

                Console.WriteLine("Persisted to DB.");

                return restaurant.Id;
            }
        }
    }
}
