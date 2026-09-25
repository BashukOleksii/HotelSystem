using Microsoft.EntityFrameworkCore;

namespace lab_01.Models.Entities
{
    [Owned]
    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
    }
}
