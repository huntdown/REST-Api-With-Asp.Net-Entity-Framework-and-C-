using Microsoft.AspNet.Identity;
using QuotesWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace QuotesWebApi.Controllers
{
    [Authorize]
    public class QuotesController : ApiController
    {
        ApplicationDbContext quotesDbContext = new ApplicationDbContext();
        // GET: api/Quotes
        [CacheOutput(ClientTimeSpan =60,ServerTimeSpan =60)]
        [AllowAnonymous]
        [HttpGet]
        
        public IHttpActionResult loadQuotes(string sort)
        {
            IQueryable<Quote> quotes;
            switch (sort)
            {
                case "desc":
                    quotes = quotesDbContext.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    quotes = quotesDbContext.Quotes.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    quotes = quotesDbContext.Quotes;
                    break;
            }
            return Ok(quotes);
        }
        [HttpGet]
        [Route("api/Quotes/MyQuotes")]
        public IHttpActionResult MyQuotes()
        {
            string userId = User.Identity.GetUserId();
            var quotes = quotesDbContext.Quotes.Where(q=>q.UserId==userId);
            return Ok(quotes);
        }
        [HttpGet]
        [Route("api/Quotes/PagingQuote/{pageNumber=}/{pageSize=}")]
        public IHttpActionResult PagingQuote(int pageNumber, int pageSize)
        {
            var quotes = quotesDbContext.Quotes.OrderBy(q=>q.Id);
            return Ok(quotes.Skip((pageNumber - 1) * pageSize));

        }
        [HttpGet]
        [Route("api/Quotes/SearchQuote/{type=}")]
        public IHttpActionResult SearchQuote(string type)
        {
            var quotes = quotesDbContext.Quotes.Where(q => q.Type.StartsWith(type));
            return Ok(quotes);
        }
        [HttpGet]
        [Route("api/Quotes/Test/{id}")]
        public int Test(int id)
        {
            return id;
        }

        // GET: api/Quotes/5
        [HttpGet]
        public IHttpActionResult loadQuote(int id)
        {
            var quote = quotesDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return NotFound();
            }
            return Ok(quote);
        }

        // POST: api/Quotes
        [HttpPost]
        public IHttpActionResult Post([FromBody]Quote quote)
        {
            string userId = User.Identity.GetUserId();
            quote.UserId = userId;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            quotesDbContext.Quotes.Add(quote);
            quotesDbContext.SaveChanges();
            return StatusCode(HttpStatusCode.Created);
        }

        // PUT: api/Quotes/5
        public IHttpActionResult Put(int id, [FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            var entity = quotesDbContext.Quotes.FirstOrDefault(q => q.Id == id);
           
            if (entity == null)
            {
                return BadRequest("No record found against this id");
            }
            if (userId != entity.UserId)
            {
                return BadRequest("You don't have right to update this record");
            }
            entity.Title = quote.Title;
            entity.Author = quote.Author;
            entity.Description = quote.Description;
            quotesDbContext.SaveChanges();
            return Ok("Record updated successfully...");
        }

        // DELETE: api/Quotes/5
        public IHttpActionResult Delete(int id)
        {
            string userId = User.Identity.GetUserId();
            var quote = quotesDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return BadRequest("No record found...");
            }
            if (userId != quote.UserId)
            {
                return BadRequest("You don't have any rights to delete this record");
            }
            quotesDbContext.Quotes.Remove(quote);
            quotesDbContext.SaveChanges();
            return Ok("Quote deleted");

        }
    }
}
