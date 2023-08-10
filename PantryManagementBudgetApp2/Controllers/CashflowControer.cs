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
    public class TagController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TagController()
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

        // GET: Tag/List
        public ActionResult List()
        {
            //objective: communicate with tag data api to retrieve a list of tags
            // curl https://localhost:44302/api/TagData/ListTags

            string url = "TagData/ListTags";

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<TagDto> Tags = response.Content.ReadAsAsync<IEnumerable<TagDto>>().Result;
            //Debug.WriteLine("Number of tag received: ");
            //Debug.WriteLine(Tags.Count());

            return View(Tags);
        }

        // GET: Tag/Details/5
        public ActionResult Details(int id)
        {
            DetailsTag ViewModel = new DetailsTag();

            //objective: communicate with tag data api to retrieve one tag
            // curl https://localhost:44302/api/TagData/FindTag/{id}

            string url = "TagData/FindTag/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            TagDto SelectedTag = response.Content.ReadAsAsync<TagDto>().Result;
            //Debug.WriteLine("tag received: ");
            //Debug.WriteLine(SelectedTag.TagName);

            ViewModel.SelectedTag = SelectedTag;


            //Show all pantry items under this tag
            url = "PantryItemData/ListPantryItemsForTag/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<PantryItemDto> CarryTags = response.Content.ReadAsAsync<IEnumerable<PantryItemDto>>().Result;

            ViewModel.CarryTags = CarryTags;

            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Tag/New
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();
            return View();
        }

        // POST: Tag/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Tag Tag)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is :");
            //objective: add a new tag into our system using the API

            string url = "TagData/AddTag";

            string jsonpayload = jss.Serialize(Tag);

            Debug.WriteLine(jsonpayload);

            //curl -H "Content-Type:application/json" -d @Tag.json https://localhost:44302/api/Tagdata/addTag 

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

        // GET: Tag/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();
            string url = "Tagdata/findTag/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TagDto selectedTag = response.Content.ReadAsAsync<TagDto>().Result;
            return View(selectedTag);
        }

        // POST: Tag/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Tag Tag)
        {
            GetApplicationCookie();
            string url = "Tagdata/updateTag/" + id;
            string jsonpayload = jss.Serialize(Tag);
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

        // GET: Tag/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "Tagdata/findTag/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TagDto selectedTag = response.Content.ReadAsAsync<TagDto>().Result;
            return View(selectedTag);
        }

        // POST: Tag/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "Tagdata/deleteTag/" + id;
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
