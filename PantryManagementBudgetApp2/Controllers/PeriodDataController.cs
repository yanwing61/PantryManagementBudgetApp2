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
    public class PeriodDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// API call that gets all periods from the database periods table
        /// </summary>
        /// <returns>
        /// PeriodId, PeriodYear, PeriodMonth values of all records in database periods table
        /// </returns>
        // GET: api/PeriodData/ListPeriods
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IEnumerable<PeriodDto> ListPeriods()
        {
            List<Period> Periods = db.Periods.ToList();
            List<PeriodDto> PeriodDtos = new List<PeriodDto>();

            Periods.ForEach(per => PeriodDtos.Add(new PeriodDto()
            {
                PeriodId = per.PeriodId,
                PeriodYear = per.PeriodYear,
                PeriodMonth = per.PeriodMonth
            }));

            return PeriodDtos;
        }

        /// <summary>
        /// API call that looks for a specific period in the periods table identified by the id input
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// PeriodId, PeriodYear & PeriodMonth of specific record identified by id input
        /// </returns>
        // GET: api/PeriodData/FindPeriod/5
        [ResponseType(typeof(Period))]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult FindPeriod(int id)
        {
            Period Period = db.Periods.Find(id);
            PeriodDto PeriodDto = new PeriodDto()
            {
                PeriodId = Period.PeriodId,
                PeriodYear = Period.PeriodYear,
                PeriodMonth = Period.PeriodMonth
            };
            if (Period == null)
            {
                return NotFound();
            }

            return Ok(PeriodDto);
        }

        // POST: api/PeriodData/UpdatePeriod/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdatePeriod(int id, Period period)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != period.PeriodId)
            {
                return BadRequest();
            }

            db.Entry(period).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeriodExists(id))
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

        // POST: api/PeriodData/AddPeriod
        [ResponseType(typeof(Period))]
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult AddPeriod(Period period)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Periods.Add(period);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = period.PeriodId }, period);
        }

        // DELETE: api/PeriodData/DeletePeriod/5
        [ResponseType(typeof(Period))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeletePeriod(int id)
        {
            Period period = db.Periods.Find(id);
            if (period == null)
            {
                return NotFound();
            }

            db.Periods.Remove(period);
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

        private bool PeriodExists(int id)
        {
            return db.Periods.Count(e => e.PeriodId == id) > 0;
        }
    }
}