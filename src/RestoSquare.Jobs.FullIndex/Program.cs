using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Microsoft.WindowsAzure;

using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;

using RestoSquare.Core;
using RestoSquare.Core.Helpers;
using RestoSquare.Data;

namespace RestoSquare.Jobs.FullIndex
{
    internal class Program
    {
        public static int Skip = 0;
        public static int Take = 500;
        public static int Processed = 0;

        private static void Main(string[] args)
        {
            var indexName = CloudConfigurationManager.GetSetting("Azure.Search.IndexName");

            Log("Starting full sync to: {0}", indexName);

            var searchClient = GetSearchClient();

            while (true)
            {
                using (var dbTimer = CallTimer.Start())
                {
                    // Get batch of restaurants.
                    var restaurants = GetRestaurants(Skip, Take);
                    dbTimer.Stop();

                    // No results, EOF.
                    if (restaurants == null)
                    {
                        break;
                    }

                    // Flatten.
                    var operations = new List<IndexOperation>();
                    foreach (var restaurant in restaurants)
                    {
                        var indexOperation = new IndexOperation(IndexOperationType.MergeOrUpload, "id", restaurant.Id.ToString());
                        indexOperation
                            .WithProperty("internalName", restaurant.InternalName)
                            .WithProperty("name", restaurant.Name)
                            .WithProperty("postalCode", restaurant.PostalCode)
                            .WithProperty("locality", restaurant.Locality)
                            .WithProperty("street", restaurant.StreetAddress)
                            .WithProperty("website", restaurant.Website)
                            .WithProperty("budget", restaurant.Budget)
                            .WithProperty("rating", restaurant.Rating)
                            .WithProperty("fax", restaurant.Fax)
                            .WithProperty("mobile", restaurant.Mobile)
                            .WithProperty("phoneNumber", restaurant.PhoneNumber)
                            .WithProperty("email", restaurant.Email)
                            .WithProperty("hasImage", restaurant.HasImage)

                            // Translated content.
                            .WithProperty("region", restaurant.Region.TryGet("en"))
                            .WithProperty("region_nl", restaurant.Region.TryGet("nl"))
                            .WithProperty("region_fr", restaurant.Region.TryGet("fr"))
                            .WithProperty("description", restaurant.TryGet(r => r.Description, "en"))
                            .WithProperty("description_fr", restaurant.TryGet(r => r.Description, "fr"))
                            .WithProperty("description_nl", restaurant.TryGet(r => r.Description, "nl"))
                            .WithProperty("closing", restaurant.TryGet(r => r.Closing, "en"))
                            .WithProperty("closing_fr", restaurant.TryGet(r => r.Closing, "fr"))
                            .WithProperty("closing_nl", restaurant.TryGet(r => r.Closing, "nl"))
                            .WithProperty("setting", restaurant.TryGet(r => r.Setting, "en"))
                            .WithProperty("setting_fr", restaurant.TryGet(r => r.Setting, "fr"))
                            .WithProperty("setting_nl", restaurant.TryGet(r => r.Setting, "nl"))

                            // Translated tags.
                            .WithProperty("accommodations", restaurant.Accommodations.Select(a => a.Accommodation).TryGet<Accommodation, AccommodationTranslation>("en"))
                            .WithProperty("accommodations_fr", restaurant.Accommodations.Select(a => a.Accommodation).TryGet<Accommodation, AccommodationTranslation>("fr"))
                            .WithProperty("accommodations_nl", restaurant.Accommodations.Select(a => a.Accommodation).TryGet<Accommodation, AccommodationTranslation>("nl"))
                            .WithProperty("cuisine", restaurant.Cuisines.Select(a => a.Cuisine).TryGet<Cuisine, CuisineTranslation>("en"))
                            .WithProperty("cuisine_fr", restaurant.Cuisines.Select(a => a.Cuisine).TryGet<Cuisine, CuisineTranslation>("fr"))
                            .WithProperty("cuisine_nl", restaurant.Cuisines.Select(a => a.Cuisine).TryGet<Cuisine, CuisineTranslation>("nl"))
                            .WithProperty("paymentFacilities", restaurant.PaymentFacilities.Select(a => a.PaymentFacility).TryGet<PaymentFacility, PaymentFacilityTranslation>("en"))
                            .WithProperty("paymentFacilities_fr", restaurant.PaymentFacilities.Select(a => a.PaymentFacility).TryGet<PaymentFacility, PaymentFacilityTranslation>("fr"))
                            .WithProperty("paymentFacilities_nl", restaurant.PaymentFacilities.Select(a => a.PaymentFacility).TryGet<PaymentFacility, PaymentFacilityTranslation>("nl"));
                        
                        // Add geocoordinates if available.
                        if (restaurant.Longitude.HasValue && restaurant.Latitude.HasValue)
                            indexOperation.WithGeographyPoint("location", restaurant.Longitude.Value, restaurant.Latitude.Value);

                        // Add to batch.
                        operations.Add(indexOperation);
                    }

                    using (var searchTimer = CallTimer.Start())
                    {
                        var response = searchClient.PopulateAsync(indexName, operations.ToArray()).Result;

                        // Error handling!
                        if (!response.IsSuccess)
                        {
                            throw new Exception(response.StatusCode.ToString());
                        }
                        else
                        {
                            var failed = response.Body.Where(r => !r.Status);
                            foreach (var item in failed)
                            {
                                Log("Failed: {0} ({1})", item.Key, item.ErrorMessage);
                            }
                        }

                        // Move forward.
                        Skip += Take;
                        Processed += restaurants.Count();

                        // Done!
                        Log("Processed: {0} (Db: {1} s., Search: {2} s.)", Processed, dbTimer.TotalSeconds, searchTimer.TotalSeconds);
                    }
                }
            }

            Log("Done!");
        }

        /// <summary>
        /// Initialize search.
        /// </summary>
        /// <returns></returns>
        public static IndexManagementClient GetSearchClient()
        {
            var connection = ApiConnection.Create(CloudConfigurationManager.GetSetting("Azure.Search.ServiceName"),
                CloudConfigurationManager.GetSetting("Azure.Search.ApiKey"));
            var indexClient = new IndexManagementClient(connection);
            return indexClient;
        }

        /// <summary>
        /// Load a batch of restaurants.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        private static IReadOnlyCollection<Restaurant> GetRestaurants(int skip, int take)
        {
            using (var ctx = new RestoContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.LazyLoadingEnabled = false;
                ctx.Configuration.ProxyCreationEnabled = false;

                var restaurants = ctx.Restaurants
                    .Include(r => r.Translations)
                    .Include(r => r.Region.Translations)
                    .Include(r => r.Cuisines.Select(a => a.Cuisine.Translations))
                    .Include(r => r.Accommodations.Select(a => a.Accommodation.Translations))
                    .Include(r => r.PaymentFacilities.Select(a => a.PaymentFacility.Translations))
                    .OrderBy(r => r.Name)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                if (restaurants.Count == 0)
                    return null;
                return restaurants.ToList();
            }
        }

        private static void Log(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}
