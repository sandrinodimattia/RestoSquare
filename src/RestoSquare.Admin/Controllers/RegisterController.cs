using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using RestoSquare.Admin.Models;
using RestoSquare.Data;
using RestoSquare.Domain;

namespace RestoSquare.Admin.Controllers
{
    public class RegisterController : Controller
    {
        public ActionResult Index()
        {
            using (var ctx = new RestoContext())
            {
                var vm = new RegisterViewModel();
                vm.Regions = ctx.RegionTranslations
                    .Where(r => r.Language == "en").ToList()
                    .OrderBy(r => r.Title)
                    .Select(r => new SelectListItem { Text = r.Title, Value = r.ParentId.ToString(CultureInfo.InvariantCulture) });
                vm.Cuisines = ctx.CuisineTranslations
                    .Where(c => c.Language == "en").ToList()
                    .OrderBy(c => c.Title)
                    .Select(c => new SelectListItem { Text = c.Title, Value = c.ParentId.ToString(CultureInfo.InvariantCulture) });
                vm.Accomodations = ctx.AccommodationTranslations
                    .Where(a => a.Language == "en")
                    .OrderBy(a => a.Title)
                    .ToList();
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult Index(RegisterViewModel model)
        {
            try
            {
                var command = new RegisterRestoCommand();
                command.Budget = model.Budget;
                command.City = model.City;
                command.Cuisine = model.Cuisine;
                command.Name = model.Name;
                command.Rating = model.Rating;
                command.Region = model.Region;
                command.SelectedAccommodationIds = model.SelectedAccommodationIds;
                command.Street = model.Street;

                var queue = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageAccount"))
                    .CreateCloudQueueClient()
                    .GetQueueReference("commands");
                queue.CreateIfNotExists();
                queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(command)));

                return View("Success");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }
    }
}