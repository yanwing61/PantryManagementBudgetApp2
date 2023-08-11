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
    public class PurchaseController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        
        static PurchaseController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44394/api/");
        }

        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            Debug.WriteLine("Token Submitted Is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Purchase/List
        [Authorize]
        public ActionResult List()
        {
            GetApplicationCookie(); //get token credentials
            // Objective: Communicate with Purchase data API to retrieve list of Purchases
            // curl https://localhost:44394/api/PurchaseData/ListPurchases

            string url = "PurchaseData/ListPurchases";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<PurchaseDto> purchases = response.Content.ReadAsAsync<IEnumerable<PurchaseDto>>().Result;

            return View(purchases);
        }

        // GET: Purchase/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            GetApplicationCookie(); //get token credentials
            // Objective: Communicate with Purchase data API to retrieve one Purchase record
            // curl https://localhost:44394/api/PurchaseData/FindPurchase/{id}

            string url = "PurchaseData/FindPurchase/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PurchaseDto selectedPurchase = response.Content.ReadAsAsync<PurchaseDto>().Result;

            return View(selectedPurchase);
        }

        // Error Page
        public ActionResult Error()
        {
            return View();
        }

        // GET: Purchase/New
        [Authorize]
        public ActionResult New()
        {
            // Info about all periods in system
            // GET: api/PeriodData/ListPeriods
            string url = "PeriodData/ListPeriods";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<PeriodDto> periodOptions = response.Content.ReadAsAsync<IEnumerable<PeriodDto>>().Result;

            return View(periodOptions);
        }

        // POST: Purchase/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Purchase purchase)
        {
            GetApplicationCookie(); //get token credentials
            Debug.WriteLine("JSON payload is:");
            // Debug.WriteLine(purchase.Inflow);

            // Objective: Add new purchase record to system using API
            // curl -H "Content-type:application/json" -d @record.json http://localhost:44394/api/PurchaseData/AddPurchase

            string url = "PurchaseData/AddPurchase";

            string jsonpayload = jss.Serialize(purchase);

            Debug.WriteLine(jsonpayload);

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

        // GET: Purchase/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            UpdatePurchase ViewModel = new UpdatePurchase();

            // existing purchase info
            string url = "PurchaseData/FindPurchase/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PurchaseDto SelectedPurchase = response.Content.ReadAsAsync<PurchaseDto>().Result;
            ViewModel.SelectedPurchase = SelectedPurchase;

            // also include all periods to choose from when updating this Purchase record
            url = "PeriodData/ListPeriods/";
            response = client.GetAsync(url).Result;
            IEnumerable<PeriodDto> PeriodOptions = response.Content.ReadAsAsync<IEnumerable<PeriodDto>>().Result;
            ViewModel.PeriodOptions = PeriodOptions;

            return View(ViewModel);
        }

        // POST: Purchase/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Purchase purchase)
        {
            GetApplicationCookie(); //get token credentials
            string url = "PurchaseData/UpdatePurchase/" + id;
            string jsonpayload = jss.Serialize(Purchase);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Purchase/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "PurchaseData/FindPurchase/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PurchaseDto selectedpurchase = response.Content.ReadAsAsync<PurchaseDto>().Result;
            return View(selectedpurchase);
        }

        // POST: Purchase/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            GetApplicationCookie(); //get token credentials
            string url = "PurchaseData/DeletePurchase/" + id;
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
