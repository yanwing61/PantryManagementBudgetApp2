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
    public class PurchaseDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Lists purchases in systyem
        /// </summary>
        /// <returns>
        /// Returns all purchase DTOs in system
        /// </returns>
        /// <example>
        /// GET: api/PurchaseData/ListPurchases
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IEnumerable<PurchaseDto> ListPurchases()
        {
            List<Purchase> Purchases = db.Purchases.ToList();
            List<PurchaseDto> PurchaseDtos = new List<PurchaseDto>();

            Purchases.ForEach(p => PurchaseDtos.Add(new PurchaseDto()
            {
                PurchaseID = p.PurchaseID,
                UnitPrice = p.UnitPrice,
                Qty = p.Qty,
                PeriodId = p.PeriodId,
                PeriodYear = p.Period.PeriodYear,
                PeriodMonth = p.Period.PeriodMonth,
                PantryItemID = p.PantryItemID,
                PantryItemName = p.PantryItem.PantryItemName,
                PantryItemCurrentQty = p.PantryItem.PantryItemCurrentQty
            }));

            return PurchaseDtos;
        }

        /// <summary>
        /// Gather info about purchases related to particular period
        /// </summary>
        /// <param name="id">Period ID</param>
        /// <returns>
        /// All records in purchase for given period
        /// </returns>
        /// <example>
        /// GET: api/PurchaseData/ListPurchasesForPeriod/1
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ResponseType(typeof(PurchaseDto))]
        public IHttpActionResult ListPurchasesForPeriod(int id)
        {
            List<Purchase> Purchases = db.Purchases.Where(p => p.PeriodId == id).ToList();
            List<PurchaseDto> PurchaseDtos = new List<PurchaseDto>();

            Purchases.ForEach(p => PurchaseDtos.Add(new PurchaseDto()
            {
                PurchaseID = p.PurchaseID,
                UnitPrice = p.UnitPrice,
                Qty = p.Qty,
                PeriodId = p.PeriodId,
                PeriodYear = p.Period.PeriodYear,
                PeriodMonth = p.Period.PeriodMonth,
                PantryItemID = p.PantryItemID,
                PantryItemName = p.PantryItem.PantryItemName,
                PantryItemCurrentQty = p.PantryItem.PantryItemCurrentQty
            }));

            return Ok(PurchaseDtos);
        }

        /// <summary>
        /// Gather info about purchases related to particular pantry item
        /// </summary>
        /// <param name="id">Pantry Item ID</param>
        /// <returns>
        /// All records in purchase for given pantry item
        /// </returns>
        /// <example>
        /// GET: api/PurchaseData/ListPurchasesForPantryItem/1
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ResponseType(typeof(PurchaseDto))]
        public IHttpActionResult ListPurchasesForPantryItem(int id)
        {
            List<Purchase> Purchases = db.Purchases.Where(p => p.PantryItemID == id).ToList();
            List<PurchaseDto> PurchaseDtos = new List<PurchaseDto>();

            Purchases.ForEach(p => PurchaseDtos.Add(new PurchaseDto()
            {
                PurchaseID = p.PurchaseID,
                UnitPrice = p.UnitPrice,
                Qty = p.Qty,
                PeriodId = p.PeriodId,
                PeriodYear = p.Period.PeriodYear,
                PeriodMonth = p.Period.PeriodMonth,
                PantryItemID = p.PantryItemID,
                PantryItemName = p.PantryItem.PantryItemName,
                PantryItemCurrentQty = p.PantryItem.PantryItemCurrentQty
            }));

            return Ok(PurchaseDtos);
        }

        /// <summary>
        /// To find a purchase record in the system
        /// </summary>
        /// <param name="id">Purchase ID</param>
        /// <returns>
        /// A purchase DTO in system based on purchase ID input
        /// </returns>
        /// <example>
        /// GET: api/PurchaseData/FindPurchase/1
        /// </example> 
        [ResponseType(typeof(Purchase))]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult FindPurchase(int id)
        {
            Purchase Purchase = db.Purchases.Find(id);
            PurchaseDto PurchaseDto = new PurchaseDto()
            {
                PurchaseID = Purchase.PurchaseID,
                UnitPrice = Purchase.UnitPrice,
                Qty = Purchase.Qty,
                PeriodId = Purchase.PeriodId,
                PeriodYear = Purchase.Period.PeriodYear,
                PeriodMonth = Purchase.Period.PeriodMonth,
                PantryItemID = Purchase.PantryItemID,
                PantryItemName = Purchase.PantryItem.PantryItemName,
                PantryItemCurrentQty = Purchase.PantryItem.PantryItemCurrentQty
            };
            if (Purchase == null)
            {
                return NotFound();
            }
            return Ok(PurchaseDto);
        }

        /// <summary>
        /// Updates particular purchase record in system with POST data input
        /// </summary>
        /// <param name="id">Purchase ID primary key</param>
        /// <param name="purchase">JSON FORM DATA of a purchase</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/PurchaseData/UpdatePurchase/5
        /// </example>
        
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdatePurchase(int id, Purchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchase.PurchaseID)
            {
                return BadRequest();
            }

            db.Entry(purchase).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds specific purchase record to system
        /// </summary>
        /// <param name="purchase">JSON FORM DATA of a purchase</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: purchase ID, Purchase Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/PurchaseData/AddPurchase
        /// </example>

        [ResponseType(typeof(Purchase))]
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult AddPurchase(Purchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Purchases.Add(purchase);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = purchase.PurchaseID }, purchase);
        }

        /// <summary>
        /// To delete purchase in system
        /// </summary>
        /// <param name="id">the purchase ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND if the id did not exist)
        /// </returns>
        /// <example>
        /// POST: api/PurchaseData/DeletePurchase/5
        /// </example>

        [ResponseType(typeof(Purchase))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeletePurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }

            db.Purchases.Remove(purchase);
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

        private bool PurchaseExists(int id)
        {
            return db.Purchases.Count(e => e.PurchaseID == id) > 0;
        }

    }
}