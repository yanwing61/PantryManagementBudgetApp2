using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PantryManagementBudgetApp2.Models;

namespace PantryManagementBudgetApp2.Controllers
{
    public class PantryItemDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// To list out all Pantry Items in the system.
        /// </summary>
        /// <returns>
        /// all Pantry Items (DTO) in the system.
        /// </returns>
        /// <example>
        /// GET: api/PantryItemData/ListPantryItems
        /// </example>
        [HttpGet]
        public IEnumerable<PantryItemDto> ListPantryItems()
        {
            List<PantryItem> PantryItems = db.PantryItems.ToList();
            List<PantryItemDto> PantryItemDtos = new List<PantryItemDto>();

            PantryItems.ForEach(p => PantryItemDtos.Add(new PantryItemDto()
            {
                PantryItemID = p.PantryItemID,
                PantryItemName = p.PantryItemName,
                PantryItemCurrentQty = p.PantryItemCurrentQty
            }));

            return PantryItemDtos;
        }


        /// <summary>
        /// Gathers information about PantryItems related to a particular Tag
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all PantryItems in the database that match to a particular Tag id
        /// </returns>
        /// <param name="id">Tag ID</param>
        /// <example>
        /// GET: api/PantryItemData/ListPantryItemsForTag/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(PantryItemDto))]
        public IHttpActionResult ListPantryItemsForTag(int id)
        {
            //all PantryItems that have Tags which match with our ID
            List<PantryItem> PantryItems = db.PantryItems.Where(
                p => p.Tags.Any(
                    t => t.TagID == id
                )).ToList();
            List<PantryItemDto> PantryItemDtos = new List<PantryItemDto>();

            PantryItems.ForEach(p => PantryItemDtos.Add(new PantryItemDto()
            {
                PantryItemID = p.PantryItemID,
                PantryItemName = p.PantryItemName,
                PantryItemCurrentQty = p.PantryItemCurrentQty
            }));

            return Ok(PantryItemDtos);
        }

        /// <summary>
        /// Associates a particular Tag with a particular PantryItem
        /// </summary>
        /// <param name="pantryitemid">The PantryItem ID primary key</param>
        /// <param name="tagid">The Tag ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/PantryItemData/AssociatePantryItemWithTag/6/1
        /// </example>
        [HttpPost]
        [Authorize]
        [Route("api/PantryItemData/AssociatePantryItemWithTag/{pantryitemid}/{tagid}")]
        public IHttpActionResult AssociatePantryItemWithTag(int pantryitemid, int tagid)
        {

            PantryItem SelectedPantryItem = db.PantryItems.Include(p => p.Tags).Where(p => p.PantryItemID == pantryitemid).FirstOrDefault();
            Tag SelectedTag = db.Tags.Find(tagid);

            if (SelectedPantryItem == null || SelectedTag == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input PantryItem id is: " + pantryitemid);
            Debug.WriteLine("selected PantryItem name is: " + SelectedPantryItem.PantryItemName);
            Debug.WriteLine("input Tag id is: " + tagid);
            Debug.WriteLine("selected Tag name is: " + SelectedTag.TagName);


            SelectedPantryItem.Tags.Add(SelectedTag);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular Tag and a particular PantryItem
        /// </summary>
        /// <param name="pantryitemid">The PantryItem ID primary key</param>
        /// <param name="tagid">The Tag ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/PantryItemData/UnAssociatePantryItemWithTag/6/1
        /// </example>
        [HttpPost]
        [Authorize]
        [Route("api/PantryItemData/UnAssociatePantryItemWithTag/{pantryitemid}/{tagid}")]
        public IHttpActionResult UnAssociatePantryItemWithTag(int pantryitemid, int tagid)
        {

            PantryItem SelectedPantryItem = db.PantryItems.Include(p => p.Tags).Where(p => p.PantryItemID == pantryitemid).FirstOrDefault();
            Tag SelectedTag = db.Tags.Find(tagid);

            if (SelectedPantryItem == null || SelectedTag == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input PantryItem id is: " + pantryitemid);
            Debug.WriteLine("selected PantryItem name is: " + SelectedPantryItem.PantryItemName);
            Debug.WriteLine("input Tag id is: " + tagid);
            Debug.WriteLine("selected Tag name is: " + SelectedTag.TagName);


            SelectedPantryItem.Tags.Remove(SelectedTag);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// To find a Pantry Item in the system.
        /// </summary>
        /// <param name="id">the Pantry Item ID primary key</param>
        /// <returns>
        /// a Pantry Item(DTO) in the system based on the pantry item id primary key input
        /// OR
        /// 404 NOT FOUND if the id did not exist
        /// </returns>
        /// <example>
        /// GET: api/PantryItemData/FindPantryItem/5
        /// </example> 
        [ResponseType(typeof(PantryItem))]
        [HttpGet]

        public IHttpActionResult FindPantryItem(int id)
        {
            PantryItem PantryItem = db.PantryItems.Find(id);
            PantryItemDto PantryItemDto = new PantryItemDto()
            {
                PantryItemID = PantryItem.PantryItemID,
                PantryItemName = PantryItem.PantryItemName,
                PantryItemCurrentQty = PantryItem.PantryItemCurrentQty
            };
            if (PantryItem == null)
            {
                return NotFound();
            }
            return Ok(PantryItemDto);
        }

        /// <summary>
        /// Updates a particular Pantry Item in the system with POST Data input
        /// </summary>
        /// <param name="id">the Pantry Item ID primary key</param>
        /// <param name="pantryItem">JSON FORM DATA of a pantry item</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/PantryItemData/UpdatePantryItem/5
        /// </example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdatePantryItem(int id, PantryItem pantryItem)
        {
            Debug.WriteLine("update method reached");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != pantryItem.PantryItemID)
            {
                Debug.WriteLine("id mismatch!");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + pantryItem.PantryItemID);
                Debug.WriteLine("POST parameter" + pantryItem.PantryItemName);
                Debug.WriteLine("POST parameter" + pantryItem.PantryItemCurrentQty);
                return BadRequest();
            }

            db.Entry(pantryItem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PantryItemExists(id))
                {
                    Debug.WriteLine("Pantry item not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("none of the condition triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Adds a particular Pantry Item to the system 
        /// </summary>
        /// <param name="pantryItem">JSON FORM DATA of a pantry item</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Pantry Item ID, Pantry Item Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/PantryItemData/AddPantryItem
        /// </example>

        [ResponseType(typeof(PantryItem))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddPantryItem(PantryItem pantryItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PantryItems.Add(pantryItem);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pantryItem.PantryItemID }, pantryItem);
        }

        /// <summary>
        /// To delete a Pantry Item in the system.
        /// </summary>
        /// <param name="id">the Pantry Item ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// OR
        /// HEADER: 404 (NOT FOUND if the id did not exist)
        /// </returns>
        /// <example>
        /// POST: api/PantryItemData/DeletePantryItem/5
        /// </example> 

        [ResponseType(typeof(PantryItem))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeletePantryItem(int id)
        {
            PantryItem pantryItem = db.PantryItems.Find(id);
            if (pantryItem == null)
            {
                return NotFound();
            }

            db.PantryItems.Remove(pantryItem);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PantryItemExists(int id)
        {
            return db.PantryItems.Count(e => e.PantryItemID == id) > 0;
        }
    }
}