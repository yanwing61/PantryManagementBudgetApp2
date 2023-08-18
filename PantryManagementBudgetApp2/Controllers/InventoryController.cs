using PantryManagementBudgetApp2.Models.ViewModels;
using PantryManagementBudgetApp2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PantryManagementBudgetApp2.Controllers
{
    public class InventoryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static InventoryController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44351/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }


        // GET: Inventory/List
        [Authorize(Roles = "Admin, User")]
        public ActionResult List()
        {
            GetApplicationCookie();
            //objective: communicate with inventory data api to retrieve a list of Inventories
            // curl https://localhost:44351/api/InventoryData/ListInventories

            string url = "InventoryData/ListInventories";

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<InventoryDto> Inventories = response.Content.ReadAsAsync<IEnumerable<InventoryDto>>().Result;
            //Debug.WriteLine("Number of inventory received: ");
            //Debug.WriteLine(Inventories.Count());

            return View(Inventories);
        }

        // GET: Inventory/Details/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Details(int id)
        {
            GetApplicationCookie();
            //objective: communicate with inventory data api to retrieve one inventory
            // curl https://localhost:44351/api/InventoryData/FindInventory/{id}

            string url = "InventoryData/FindInventory/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            InventoryDto selectedInventory = response.Content.ReadAsAsync<InventoryDto>().Result;
            //Debug.WriteLine("inventory received: ");
            //Debug.WriteLine(selectedInventory.InventoryName);


            return View(selectedInventory);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Inventory/New
        [Authorize(Roles = "Admin, User")]
        public ActionResult New()
        {
            GetApplicationCookie();

            //information about all pantry items in the systems
            // curl https://localhost:44351/api/PantryItemData/ListPantryItems

            string url = "PantryItemData/ListPantryItems";

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<PantryItemDto> PantryItemsOptions = response.Content.ReadAsAsync<IEnumerable<PantryItemDto>>().Result;

            return View(PantryItemsOptions);
        }

        // POST: Inventory/Create
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create(Inventory inventory, int clientTimezoneOffset)
        {
            GetApplicationCookie();

            inventory.InventoryLogDate = inventory.InventoryLogDate.AddMinutes(-clientTimezoneOffset);

            Debug.WriteLine("the json payload is :");
            //objective: add a new inventory into our system using the API

            string url = "InventoryData/AddInventory";

            string jsonpayload = jss.Serialize(inventory);

            Debug.WriteLine(jsonpayload);

            //curl -H "Content-Type:application/json" -d @Inventory.json https://localhost:44351/api/Inventorydata/addInventory 

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                Debug.WriteLine(response);
                return RedirectToAction("Error");

            }

        }

        // GET: Inventory/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();
            UpdateInventory ViewModel = new UpdateInventory();

            string url = "InventoryData/FindInventory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            InventoryDto SelectedInventory = response.Content.ReadAsAsync<InventoryDto>().Result;
            ViewModel.SelectedInventory = SelectedInventory;

            url = "PantryItemData/ListPantryItems/";
            response = client.GetAsync(url).Result;
            IEnumerable<PantryItemDto> PantryItemOptions = response.Content.ReadAsAsync<IEnumerable<PantryItemDto>>().Result;

            ViewModel.PantryItemOptions = PantryItemOptions;

            return View(ViewModel);
        }

        // POST: Inventory/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Inventory inventory, int clientTimezoneOffset)
        {
            GetApplicationCookie();
            inventory.InventoryLogDate = inventory.InventoryLogDate.AddMinutes(-clientTimezoneOffset);
            string url = "Inventorydata/updateInventory/" + id;
            string jsonpayload = jss.Serialize(inventory);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            Debug.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                Debug.WriteLine(response);
                return RedirectToAction("Error");
            }
        }

        // GET: Inventory/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "Inventorydata/findInventory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            InventoryDto selectedInventory = response.Content.ReadAsAsync<InventoryDto>().Result;
            Debug.WriteLine(response);
            return View(selectedInventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "Inventorydata/deleteInventory/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                Debug.WriteLine(response);
                return RedirectToAction("Error");
            }
        }
    }
}
