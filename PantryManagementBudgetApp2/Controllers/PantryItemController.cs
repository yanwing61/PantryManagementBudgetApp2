using PantryManagementBudgetApp2.Models;
using PantryManagementBudgetApp2.Models.ViewModels;
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
    public class PantryItemController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static PantryItemController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44302/api/");
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

        // GET: PantryItem/List
        public ActionResult List()
        {
            //objective: communicate with pantry item data api to retrieve a list of pantry items
            // curl https://localhost:44302/api/PantryItemData/ListPantryItems

            string url = "PantryItemData/ListPantryItems";

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<PantryItemDto> pantryitems = response.Content.ReadAsAsync<IEnumerable<PantryItemDto>>().Result;
            //Debug.WriteLine("Number of pantry item received: ");
            //Debug.WriteLine(pantryitems.Count());

            return View(pantryitems);
        }

        // GET: PantryItem/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with pantry item data api to retrieve one pantry item
            // curl https://localhost:44302/api/PantryItemData/FindPantryItem/{id}

            DetailsPantryItem ViewModel = new DetailsPantryItem();

            string url = "PantryItemData/FindPantryItem/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            PantryItemDto SelectedPantryItem = response.Content.ReadAsAsync<PantryItemDto>().Result;
            //Debug.WriteLine("pantry item received: ");
            //Debug.WriteLine(SelectedPantryItem.PantryItemName);

            ViewModel.SelectedPantryItem = SelectedPantryItem;

            //show associated tags with this pantry item
            url = "TagData/ListTagsForPantryItem/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TagDto> AssociatedTags = response.Content.ReadAsAsync<IEnumerable<TagDto>>().Result;

            ViewModel.AssociatedTags = AssociatedTags;

            url = "TagData/ListTagsNotAssociateWithPantryItem/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TagDto> NotAssociatedTags = response.Content.ReadAsAsync<IEnumerable<TagDto>>().Result;

            ViewModel.NotAssociatedTags = NotAssociatedTags;

            //Showcase all the inventory related to this pantry item
            //send a request to gather info about inventory realted to a specific pantry item id

            url = "InventoryData/ListInventoriesForPantryItem/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<InventoryDto> RelatedInventories = response.Content.ReadAsAsync<IEnumerable<InventoryDto>>().Result;

            ViewModel.RelatedInventories = RelatedInventories;

            return View(ViewModel);
        }

        //POST: /PantryItem/Associate/{pantryitemid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int TagID)
        {
            GetApplicationCookie();

            Debug.WriteLine("Trying to associate pantry item :" + id + " with tag " + TagID);

            //call api to associate pantry item with tag
            string url = "PantryItemData/AssociatePantryItemWithTag/" + id + "/" + TagID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //GET: /PantryItem/Unassociate/{id}?TagID={tagid}
        [HttpGet]
        [Authorize]
        public ActionResult Unassociate(int id, int TagID)
        {
            GetApplicationCookie();

            Debug.WriteLine("Trying to unassociate pantry item :" + id + " with tag " + TagID);

            //call api to associate pantry item with tag
            string url = "PantryItemData/UnassociatePantryItemWithTag/" + id + "/" + TagID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: PantryItem/New
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();
            return View();
        }

        // POST: PantryItem/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(PantryItem pantryItem)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is :");
            //objective: add a new Pantry Item into our system using the API

            string url = "PantryItemData/AddPantryItem";

            string jsonpayload = jss.Serialize(pantryItem);

            Debug.WriteLine(jsonpayload);

            //curl -H "Content-Type:application/json" -d @PantryItem.json https://localhost:44302/api/PantryItemdata/addPantryItem 

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: PantryItem/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();
            string url = "PantryItemData/findpantryitem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PantryItemDto selectedPantryItem = response.Content.ReadAsAsync<PantryItemDto>().Result;
            return View(selectedPantryItem);
        }

        // POST: PantryItem/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, PantryItem PantryItem)
        {
            GetApplicationCookie();
            string url = "PantryItemdata/updatePantryItem/" + id;
            string jsonpayload = jss.Serialize(PantryItem);
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
                return RedirectToAction("Error");
            }
        }

        // GET: PantryItem/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "PantryItemdata/findPantryItem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PantryItemDto selectedPantryItem = response.Content.ReadAsAsync<PantryItemDto>().Result;
            return View(selectedPantryItem);
        }

        // POST: PantryItem/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "PantryItemdata/deletePantryItem/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}