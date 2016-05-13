using CORE.Models;
using System.Collections.Generic;

namespace CORE.Services
{
    public interface IService
    {
        void SaveBook(Book book);
        Book ReadBookWithId(System.Guid id);
        IEnumerable<Book> ReadAllBooks();
        IEnumerable<Book> ReadBooksByPublisYear(int publishYear);
        IEnumerable<Book> ReadBooksByPublisher(string author);
    }
}
