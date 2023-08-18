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
    public class BalanceController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static BalanceController()
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

        // GET: Balance/List
        [Authorize(Roles = "Admin, User")]
        public ActionResult List()
        {
            GetApplicationCookie(); //get token credentials
            // Objective: Communicate with balance data API to retrieve list of balances
            // curl https://localhost:44351/api/BalanceData/ListBalances

            string url = "BalanceData/ListBalances";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<BalanceDto> balances = response.Content.ReadAsAsync<IEnumerable<BalanceDto>>().Result;

            return View(balances);
        }

        // GET: Balance/Details/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Details(int id)
        {
            GetApplicationCookie(); //get token credentials
            // Objective: Communicate with balance data API to retrieve one balance record
            // curl https://localhost:44351/api/BalanceData/FindBalance/{id}

            string url = "BalanceData/FindBalance/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BalanceDto selectedBalance = response.Content.ReadAsAsync<BalanceDto>().Result;

            return View(selectedBalance);
        }

        // Error Page
        public ActionResult Error()
        {
            return View();
        }

        // GET: Balance/New
        [Authorize(Roles = "Admin, User")]
        public ActionResult New()
        {
            // info about all periods in system
            // GET: api/PeriodData/ListPeriods

            string url = "PeriodData/ListPeriods";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<PeriodDto> periodOptions = response.Content.ReadAsAsync<IEnumerable<PeriodDto>>().Result;

            return View(periodOptions);
        }

        // POST: Balance/Create
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create(Balance balance)
        {
            GetApplicationCookie(); //get token credentials
            Debug.WriteLine("JSON payload is:");
            // Debug.WriteLine(balance.OwnBalance);

            // Objective: Add new balance record to system using API
            // curl -H "Content-Type:application/json" -d @balance.json https://localhost:44351/api/BalanceData/AddBalance

            string url = "BalanceData/AddBalance";

            string jsonpayload = jss.Serialize(balance);

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

        // GET: Balance/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateBalance ViewModel = new UpdateBalance();

            // existing balance info
            string url = "BalanceData/FindBalance/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BalanceDto SelectedBalance = response.Content.ReadAsAsync<BalanceDto>().Result;
            ViewModel.SelectedBalance = SelectedBalance;

            // also include all periods to choose from when updating this balance record
            url = "PeriodData/ListPeriods/";
            response = client.GetAsync(url).Result;
            IEnumerable<PeriodDto> PeriodOptions = response.Content.ReadAsAsync<IEnumerable<PeriodDto>>().Result;
            ViewModel.PeriodOptions = PeriodOptions;

            return View(ViewModel);
        }

        // POST: Balance/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Balance balance)
        {
            GetApplicationCookie(); //get token credentials
            string url = "BalanceData/UpdateBalance/" + id;
            string jsonpayload = jss.Serialize(balance);
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

        // GET: Balance/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "BalanceData/FindBalance/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BalanceDto selectedbalance = response.Content.ReadAsAsync<BalanceDto>().Result;
            return View(selectedbalance);
        }

        // POST: Balance/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie(); //get token credentials
            string url = "BalanceData/DeleteBalance/" + id;
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
