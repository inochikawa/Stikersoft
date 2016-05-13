using CORE.Models;
using CORE.Services;
using System.Collections.Generic;
using System.Web.Http;
using WEB.Infrastructure.Filters;
using WEB.ViewModels;

namespace WEB.Controllers
{
    [DebugActionWebApiFilter]
    public class BookController : ApiController
    {
        private IService _bookSevice;

        public BookController(IService service)
        {
            _bookSevice = service;
        }

        // GET: api/Book
        public IEnumerable<Book> GetAllBooks()
        {
            return _bookSevice.ReadAllBooks();
        }
        
        //GET: api/Book/{id}
        public IHttpActionResult GetBook(System.Guid id)
        {
            var book = _bookSevice.ReadBookWithId(id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }
        
        //GET: api/Book/GetBooksByPublishYear/1996
        public IEnumerable<Book> GetBooksByPublishYear(int publishYear)
        {
            return _bookSevice.ReadBooksByPublisYear(publishYear);
        }

        //POST: api/Book
        public IHttpActionResult PostBook(BookViewModel model)
        {
            Book book = new Book
            {
                Name = model.Name,
                Publisher = model.Publisher,
                PublishYear = model.PublishYear
            };

            _bookSevice.SaveBook(book);
            return Ok(true);
        }
    }
}
