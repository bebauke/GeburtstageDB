using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geburtstage.Models
{
    public class vCard
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public vCardAddress Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }

    public class vCardAddress
    {
        public string Label { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
