using System.Collections.Generic;

namespace Geburtstage.Models
{
    public class Contact
    {
        public string Id { get; set; }
        public List<LabeledValue<Address>> Addresses { get; set; } = new List<LabeledValue<Address>>();
        public List<LabeledValue<string>> PhoneNumbers { get; set; } = new List<LabeledValue<string>>();
        public List<LabeledValue<string>> Emails { get; set; } = new List<LabeledValue<string>>();
        public List<LabeledValue<string>> URLs { get; set; } = new List<LabeledValue<string>>();
    }

    public class LabeledValue<T>
    {
        public string Label { get; set; }
        public T Value { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string FullAddress => $"{Street}, {PostalCode} {City}, {Country}";
        public override string ToString() => FullAddress;
    }
    
}