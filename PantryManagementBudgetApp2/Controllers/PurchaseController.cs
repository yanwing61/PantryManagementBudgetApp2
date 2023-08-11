using PantryManagementBudgetApp2.Models.ViewModels;
using PantryManagementBudgetApp2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Resources;
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
            // Objective: Communicate with purchase data API to retrieve list of purchases
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
            // Objective: Communicate with purchase data API to retrieve one purchase record
            // curl https://localhost:44394/api/PurchaseData/FindPurchase/{id}

            DetailsPurchase ViewModel = new DetailsPurchase();

            string url = "PurchaseData/FindPurchase/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PurchaseDto SelectedPurchase = response.Content.ReadAsAsync<PurchaseDto>().Result;

            ViewModel.SelectedPurchase = SelectedPurchase;
            //showcase info about balances related to this purchase
            //send a request to gather info about balances related to particular purchase ID
            url = "BalanceData/ListBalancesForPurchase/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<BalanceDto> RelatedBalances = response.Content.ReadAsAsync<IEnumerable<BalanceDto>>().Result;

            ViewModel.RelatedBalances = RelatedBalances;

            //showcase info about cashflows related to this purchase
            //send aa requeast to gather info about cashflows relted to particular purchase ID
            url = "CashflowData/ListCashflowsForPurchase/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CashflowDto> RelatedCashflows = response.Content.ReadAsAsync<IEnumerable<CashflowDto>>().Result;

            ViewModel.RelatedCashflows = RelatedCashflows;

            return View(ViewModel);
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
            return View();
        }

        // POST: Purchase/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Purchase purchase)
        {
            GetApplicationCookie(); //get token credentials
            Debug.WriteLine("JSON payload is:");
            // Debug.WriteLine(purchase.PurchaseMonth)

            // Objective: Add new purchase record to system using API
            // curl -H "Content-Type:application/json" -d @purchase.json https://localhost:44394/api/PurchaseData/AddPurchase

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
            string url = "PurchaseData/FindPurchase/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            PurchaseDto selectedpurchase = response.Content.ReadAsAsync<PurchaseDto>().Result;

            return View(selectedpurchase);
        }

        // POST: Purchase/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Purchase purchase)
        {
            GetApplicationCookie(); //get token credentials 
            string url = "PurchaseData/UpdatePurchase/" + id;
            string jsonpayload = jss.Serialize(purchase);
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
        public ActionResult Delete(int id)
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

        

        /*
        
        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }

        // GET: Purchase/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Purchase/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Purchase/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Purchase/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Purchase/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Purchase/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Purchase/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        */
    }
}
