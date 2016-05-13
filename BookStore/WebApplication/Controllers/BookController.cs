using CORE;
using CORE.Services;
using DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication.Controllers
{
    public class BookController : ApiController
    {
        private IService _bookSevice;

        public BookController(IService service)
        {
            _bookSevice = service;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _bookSevice.ReadAllBooks();
            //return BookService.ReadBooksByPublisYear(1996);
        }

        //public IHttpActionResult GetBookByPublishYear(int publishYear)
        //{
        //    var book = BookService.ReadBooksByPublisYear(publishYear);

        //    if (book == null)
        //        return NotFound();

        //    return Ok(book.First());
        //}
    }
}
