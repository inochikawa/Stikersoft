using DATA;
using System.Linq;
using System.Collections.Generic;
using CORE.Models;

namespace CORE.Services
{
    public class BookService : IService
    {
        private IClassMap _classMap;

        public BookService(IClassMap classmap)
        {
            _classMap = classmap;
        }

        public Book ReadBookWithId(System.Guid id)
        {
            return _classMap.ReadWithCondition<Book>("Id", id).FirstOrDefault();
        }

        public IEnumerable<Book> ReadBooksByPublisher(string publisher)
        {
            return _classMap.ReadWithCondition<Book>("Publisher", publisher);
        }

        public IEnumerable<Book> ReadBooksByPublisYear(int publishYear)
        {
            return _classMap.ReadWithCondition<Book>("PublishYear", publishYear);
        }

        public IEnumerable<Book> ReadAllBooks()
        {
            return _classMap.ReadAll<Book>();
        }

        public void SaveBook(Book book)
        {
            _classMap.Save<Book>(book);
        }
    }
}
