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
    public class TagDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// To list out all Tags in the system.
        /// </summary>
        /// <returns>
        /// all Tags (DTO) in the system.
        /// </returns>
        /// <example>
        /// GET: api/TagData/ListTags
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IEnumerable<TagDto> ListTags()
        {
            List<Tag> Tags = db.Tags.ToList();
            List<TagDto> TagDtos = new List<TagDto>();

            Tags.ForEach(p => TagDtos.Add(new TagDto()
            {
                TagID = p.TagID,
                TagName = p.TagName,
            }));

            return TagDtos;
        }

        /// <summary>
        /// Returns all Tags in the system associated with a particular PantryItem.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Tags in the database taking care of a particular PantryItem
        /// </returns>
        /// <param name="id">PantryItem Primary Key</param>
        /// <example>
        /// GET: api/TagData/ListTagsForPantryItem/1
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ResponseType(typeof(TagDto))]
        public IHttpActionResult ListTagsForPantryItem(int id)
        {
            List<Tag> Tags = db.Tags.Where(
                t => t.PantryItems.Any(
                    p => p.PantryItemID == id)
                ).ToList();
            List<TagDto> TagDtos = new List<TagDto>();

            Tags.ForEach(t => TagDtos.Add(new TagDto()
            {
                TagID = t.TagID,
                TagName = t.TagName,

            }));

            return Ok(TagDtos);
        }

        /// <summary>
        /// Returns all Tags in the system not associated with a particular PantryItem.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Tags in the database not associated with a particular PantryItem
        /// </returns>
        /// <param name="id">PantryItem Primary Key</param>
        /// <example>
        /// GET: api/TagData/ListTagsNotAssociateWithPantryItem/6
        /// </example>
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        [ResponseType(typeof(TagDto))]
        public IHttpActionResult ListTagsNotAssociateWithPantryItem(int id)
        {
            List<Tag> Tags = db.Tags.Where(
                t => !t.PantryItems.Any(
                    p => p.PantryItemID == id)
                ).ToList();
            List<TagDto> TagDtos = new List<TagDto>();

            Tags.ForEach(t => TagDtos.Add(new TagDto()
            {
                TagID = t.TagID,
                TagName = t.TagName,
            }));

            return Ok(TagDtos);
        }



        /// <summary>
        /// To find a Tag in the system.
        /// </summary>
        /// <param name="id">the Tag ID primary key</param>
        /// <returns>
        /// a Tag(DTO) in the system based on the Tag id primary key input
        /// OR
        /// 404 NOT FOUND if the id did not exist
        /// </returns>
        /// <example>
        /// GET: api/TagData/FindTag/5
        /// </example> 
        [ResponseType(typeof(Tag))]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult FindTag(int id)
        {
            Tag Tag = db.Tags.Find(id);
            TagDto TagDto = new TagDto()
            {
                TagID = Tag.TagID,
                TagName = Tag.TagName,
            };
            if (Tag == null)
            {
                return NotFound();
            }
            return Ok(TagDto);
        }

        /// <summary>
        /// Updates a particular Tag in the system with POST Data input
        /// </summary>
        /// <param name="id">the Tag ID primary key</param>
        /// <param name="Tag">JSON FORM DATA of a Tag</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/TagData/UpdateTag/5
        /// </example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateTag(int id, Tag Tag)
        {
            Debug.WriteLine("update method reached");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != Tag.TagID)
            {
                Debug.WriteLine("id mismatch!");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + Tag.TagID);
                Debug.WriteLine("POST parameter" + Tag.TagName);
                return BadRequest();
            }

            db.Entry(Tag).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
                {
                    Debug.WriteLine("Tag not found");
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
        /// Adds a particular Tag to the system 
        /// </summary>
        /// <param name="Tag">JSON FORM DATA of a Tag</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Tag ID, Tag Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/TagData/AddTag
        /// </example>

        [ResponseType(typeof(Tag))]
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult AddTag(Tag Tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tags.Add(Tag);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Tag.TagID }, Tag);
        }

        /// <summary>
        /// To delete a Tag in the system.
        /// </summary>
        /// <param name="id">the Tag ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// OR
        /// HEADER: 404 (NOT FOUND if the id did not exist)
        /// </returns>
        /// <example>
        /// POST: api/TagData/DeleteTag/5
        /// </example> 

        [ResponseType(typeof(Tag))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteTag(int id)
        {
            Tag Tag = db.Tags.Find(id);
            if (Tag == null)
            {
                return NotFound();
            }

            db.Tags.Remove(Tag);
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

        private bool TagExists(int id)
        {
            return db.Tags.Count(e => e.TagID == id) > 0;
        }
    }
}