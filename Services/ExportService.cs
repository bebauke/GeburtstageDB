using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geburtstage.Models;

namespace Geburtstage.Services
{
    public class ExportService
    {
        public void ExportToVCard(List<Person> persons, string path)
        {
            foreach (var person in persons)
            {
                var vCard = new vCard
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    BirthDate = person.DateOfBirth,
                    Address = person.Contact?.Addresses?.Count > 0 ? new vCardAddress
                    {
                        Label = person.Contact.Addresses[0].Label,
                        Street = person.Contact.Addresses[0].Value.Street,
                        City = person.Contact.Addresses[0].Value.City,
                        PostalCode = person.Contact.Addresses[0].Value.PostalCode,
                        Country = person.Contact.Addresses[0].Value.Country
                    } : null,
                    Phone = person.Contact?.PhoneNumbers?.Count > 0 ? person.Contact.PhoneNumbers[0].Value : null,
                    Email = person.Contact?.Emails?.Count > 0 ? person.Contact.Emails[0].Value : null,
                    Url = person.Contact?.URLs?.Count > 0 ? person.Contact.URLs[0].Value : null
                };

                var serializer = new vCardSerializer();
                var vCardString = serializer.SerializeToString(vCard);

                var filePath = Path.Combine(path, $"{person.FullName}.vcf");

                // Sicherstellen, dass das Verzeichnis existiert
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, vCardString);
            }
        }

        public void ExportToCsv(List<Person> persons, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                // CSV-Header
                writer.WriteLine("FirstName,LastName,BirthName,Prefix,Suffix,DateOfBirth,PlaceOfBirth,DateOfDeath,PlaceOfDeath,Street,City,PostalCode,Country,Email,Phone,Url");

                foreach (var person in persons)
                {
                    var address = person.Contact?.Addresses?.FirstOrDefault()?.Value;
                    var email = person.Contact?.Emails?.FirstOrDefault()?.Value ?? string.Empty;
                    var phone = person.Contact?.PhoneNumbers?.FirstOrDefault()?.Value ?? string.Empty;
                    var url = person.Contact?.URLs?.FirstOrDefault()?.Value ?? string.Empty;

                    // CSV-Zeile für jede Person
                    writer.WriteLine($"{person.FirstName},{person.LastName},{person.BirthName},{person.Prefix},{person.Suffix},{person.DateOfBirth:yyyy-MM-dd},{person.PlaceOfBirth},{person.DateOfDeath:yyyy-MM-dd},{person.PlaceOfDeath},{address?.Street},{address?.City},{address?.PostalCode},{address?.Country},{email},{phone},{url}");
                }
            }
        }

    }


    public class vCardSerializer
    {
        public string SerializeToString(vCard vCard)
        {
            return $"BEGIN:VCARD\n" +
                   $"VERSION:3.0\n" +
                   $"N:{vCard.LastName};{vCard.FirstName};;;\n" +
                   $"FN:{vCard.FirstName} {vCard.LastName}\n" +
                   (vCard.BirthDate != null ? $"BDAY:{vCard.BirthDate:yyyy-MM-dd}\n" : "") +
                   (vCard.Address != null ? $"ADR;TYPE={vCard.Address.Label}:;;{vCard.Address.Street};{vCard.Address.City};{vCard.Address.PostalCode};{vCard.Address.Country}\n" : "") +
                   (!string.IsNullOrEmpty(vCard.Phone) ? $"TEL;TYPE=HOME:{vCard.Phone}\n" : "") +
                   (!string.IsNullOrEmpty(vCard.Email) ? $"EMAIL:{vCard.Email}\n" : "") +
                   (!string.IsNullOrEmpty(vCard.Url) ? $"URL:{vCard.Url}\n" : "") +
                   $"END:VCARD";
        }
    }

}
