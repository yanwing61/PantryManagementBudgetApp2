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
    public class InventoryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// To list out all Inventories in the system.
        /// </summary>
        /// <returns>
        /// all Inventories (DTO) in the system.
        /// </returns>
        /// <example>
        /// GET: api/InventoryData/ListInventories
        /// </example>
        [HttpGet]
        public IEnumerable<InventoryDto> ListInventories()
        {
            List<Inventory> Inventories = db.Inventories.ToList();
            List<InventoryDto> InventoryDtos = new List<InventoryDto>();

            Inventories.ForEach(p => InventoryDtos.Add(new InventoryDto()
            {
                InventoryID = p.InventoryID,
                InventoryQty = p.InventoryQty,
                InventoryLogDate = p.InventoryLogDate,
                PantryItemID = p.PantryItemID,
                PantryItemName = p.PantryItem.PantryItemName,
            }));

            return InventoryDtos;
        }

        /// <summary>
        /// Gathers information about all Inventories related to a particular PantryItem ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Inventories in the database, including their associated PantryItem matched with a particular PantryItem ID
        /// </returns>
        /// <param name="id">PantryItem ID</param>
        /// <example>
        /// GET: api/InventoryData/ListInventoriesForPantryItem/6
        /// </example>
        [HttpGet]
        [ResponseType(typeof(InventoryDto))]
        public IHttpActionResult ListInventoriesForPantryItem(int id)
        {
            List<Inventory> Inventories = db.Inventories.Where(i => i.PantryItemID == id).ToList();
            List<InventoryDto> InventoryDtos = new List<InventoryDto>();

            Inventories.ForEach(i => InventoryDtos.Add(new InventoryDto()
            {
                InventoryID = i.InventoryID,
                InventoryQty = i.InventoryQty,
                InventoryLogDate = i.InventoryLogDate,
                PantryItemID = i.PantryItem.PantryItemID,
                PantryItemName = i.PantryItem.PantryItemName
            }));

            return Ok(InventoryDtos);
        }




        /// <summary>
        /// To find an inventory in the system.
        /// </summary>
        /// <param name="id">the inventory ID primary key</param>
        /// <returns>
        /// an inventory(DTO) in the system based on the inventory id primary key input
        /// OR
        /// 404 NOT FOUND if the id did not exist
        /// </returns>
        /// <example>
        /// GET: api/InventoryData/FindInventory/2
        /// </example> 
        [ResponseType(typeof(Inventory))]
        [HttpGet]
        public IHttpActionResult FindInventory(int id)
        {
            Inventory Inventory = db.Inventories.Find(id);
            InventoryDto InventoryDto = new InventoryDto()
            {
                InventoryID = Inventory.InventoryID,
                InventoryQty = Inventory.InventoryQty,
                InventoryLogDate = Inventory.InventoryLogDate,
                PantryItemID = Inventory.PantryItemID,
                PantryItemName = Inventory.PantryItem.PantryItemName
            };
            if (Inventory == null)
            {
                return NotFound();
            }
            return Ok(InventoryDto);
        }

        /// <summary>
        /// Updates a particular inventory in the system with POST Data input
        /// </summary>
        /// <param name="id">the inventory ID primary key</param>
        /// <param name="Inventory">JSON FORM DATA of an inventory</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/InventoryData/UpdateInventory/5
        /// </example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateInventory(int id, Inventory Inventory)
        {
            Debug.WriteLine("update method reached");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != Inventory.InventoryID)
            {
                Debug.WriteLine("id mismatch!");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + Inventory.InventoryID);
                Debug.WriteLine("POST parameter" + Inventory.InventoryQty);
                Debug.WriteLine("POST parameter" + Inventory.InventoryLogDate);
                Debug.WriteLine("POST parameter" + Inventory.PantryItemID);
                return BadRequest();
            }

            db.Entry(Inventory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(id))
                {
                    Debug.WriteLine("inventory not found");
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
        /// Adds a particular inventory to the system 
        /// </summary>
        /// <param name="Inventory">JSON FORM DATA of an inventory</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: inventory ID, inventory Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/InventoryData/AddInventory
        /// </example>

        [ResponseType(typeof(Inventory))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddInventory(Inventory Inventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Inventories.Add(Inventory);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Inventory.InventoryID }, Inventory);
        }

        /// <summary>
        /// To delete an inventory in the system.
        /// </summary>
        /// <param name="id">the inventory ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// OR
        /// HEADER: 404 (NOT FOUND if the id did not exist)
        /// </returns>
        /// <example>
        /// POST: api/InventoryData/DeleteInventory/5
        /// </example> 

        [ResponseType(typeof(Inventory))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteInventory(int id)
        {
            Inventory Inventory = db.Inventories.Find(id);
            if (Inventory == null)
            {
                return NotFound();
            }

            db.Inventories.Remove(Inventory);
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

        private bool InventoryExists(int id)
        {
            return db.Inventories.Count(e => e.InventoryID == id) > 0;
        }
    }
}