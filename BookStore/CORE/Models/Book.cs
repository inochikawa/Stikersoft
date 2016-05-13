using DATA;
using DATA.Attributes;

namespace CORE.Models
{   
    [Table(Name = "Book")]
    public class Book : IEntity
    {
        [Id]
        public System.Guid Id { get; set; }
        [Map]
        public string Name { get; set; }
        [Map]
        public string Publisher { get; set; }
        [Map]
        public int PublishYear { get; set; }

        public Book()
        {
            Id = System.Guid.NewGuid();
        }
    }
}
