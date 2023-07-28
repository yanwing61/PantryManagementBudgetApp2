using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        //api/PurchaseData/ListPurchases
        [HttpGet]
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

        //GET: api/PurchaseData/ListPurchasesForPeriod/1
        [HttpGet]
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

        //GET: api/PurchaseData/ListPurchasesForPantryItem/1
        [HttpGet]
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

        /// <example>
        /// GET: api/PurchaseData/FindPurchase/1
        /// </example> 
        [ResponseType(typeof(Purchase))]
        [HttpGet]
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

        /*
        // GET: api/PurchaseData
        public IQueryable<Purchase> GetPurchases()
        {
            return db.Purchases;
        }

        // GET: api/PurchaseData/5
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult GetPurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        // PUT: api/PurchaseData/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchase(int id, Purchase purchase)
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

        // POST: api/PurchaseData
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult PostPurchase(Purchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Purchases.Add(purchase);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = purchase.PurchaseID }, purchase);
        }

        // DELETE: api/PurchaseData/5
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult DeletePurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }

            db.Purchases.Remove(purchase);
            db.SaveChanges();

            return Ok(purchase);
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
        */
    }
}