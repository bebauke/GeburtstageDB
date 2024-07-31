using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Geburtstage.Models;

namespace Geburtstage.Services
{
    public class ImportService
    {
        private readonly PersonService _personService;

        public ImportService(PersonService personService)
        {
            _personService = personService;
        }

        public List<Person> ImportFromVCard(IEnumerable<string> pathsOrFolder)
        {
            var persons = new List<Person>();

            foreach (var pathOrFolder in pathsOrFolder)
            {
                if (Directory.Exists(pathOrFolder))
                {
                    // Wenn ein Ordnerpfad gegeben ist, alle Dateien durchsuchen
                    var files = Directory.GetFiles(pathOrFolder, "*.vcf", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var person = ImportSingleVCard(file);
                        if (person != null)
                        {
                            persons.Add(person);
                        }
                    }
                }
                else if (File.Exists(pathOrFolder))
                {
                    // Wenn ein Dateipfad gegeben ist, direkt importieren
                    var person = ImportSingleVCard(pathOrFolder);
                    if (person != null)
                    {
                        persons.Add(person);
                    }
                }
            }

            return persons;
        }

        private Person ImportSingleVCard(string filePath)
        {
            try
            {

                var vCardString = File.ReadAllText(filePath);
                var vCard = new vCardDeserializer().Deserialize(vCardString);

                if (vCard == null)
                {
                    throw new Exception("Fehler beim Deserialisieren der VCard.");
                }

                var person = new Person
                {
                    FirstName = vCard.FirstName,
                    LastName = vCard.LastName,
                    DateOfBirth = vCard.BirthDate,
                    Contact = new Contact()
                };

                if (vCard.Address != null)
                {
                    person.Contact.Addresses = new List<LabeledValue<Address>>
                {
                    new LabeledValue<Address>
                    {
                        Label = vCard.Address.Label,
                        Value = new Address
                        {
                            Street = vCard.Address.Street,
                            City = vCard.Address.City,
                            PostalCode = vCard.Address.PostalCode,
                            Country = vCard.Address.Country
                        }
                    }
                };
                }

                if (!string.IsNullOrEmpty(vCard.Phone))
                {
                    person.Contact.PhoneNumbers = new List<LabeledValue<string>>
                {
                    new LabeledValue<string> { Label = "Phone", Value = vCard.Phone }
                };
                }

                if (!string.IsNullOrEmpty(vCard.Email))
                {
                    person.Contact.Emails = new List<LabeledValue<string>>
                {
                    new LabeledValue<string> { Label = "Email", Value = vCard.Email }
                };
                }

                if (!string.IsNullOrEmpty(vCard.Url))
                {
                    person.Contact.URLs = new List<LabeledValue<string>>
                {
                    new LabeledValue<string> { Label = "URL", Value = vCard.Url }
                };
                }

                return person;
            }
            catch (Exception ex)
            {
                // Hier könnte eine spezifische Protokollierung oder Fehlermeldung hinzugefügt werden
                MessageBox.Show($"Fehler beim Importieren der VCard: {ex.Message}", "Importfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<Person> ImportFromCsv(string filePath)
        {
            var persons = new List<Person>();

            using (var reader = new StreamReader(filePath))
            {
                // Überspringen des Headers
                reader.ReadLine();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');

                    // Erstellen der Person
                    var person = new Person
                    {
                        FirstName = values[0],
                        LastName = values[1],
                        BirthName = values[2],
                        Prefix = values[3],
                        Suffix = values[4],
                        DateOfBirth = (DateTime)(DateTime.TryParse(values[5], out var dob) ? dob : (DateTime?)null),
                        PlaceOfBirth = values[6],
                        DateOfDeath = DateTime.TryParse(values[7], out var dod) ? dod : (DateTime?)null,
                        PlaceOfDeath = values[8],
                        Contact = new Contact
                        {
                            Addresses = new List<LabeledValue<Address>>
                            {
                                new LabeledValue<Address>
                                {
                                    Label = "Home",
                                    Value = new Address
                                    {
                                        Street = values[9],
                                        City = values[10],
                                        PostalCode = values[11],
                                        Country = values[12]
                                    }
                                }
                            },
                            Emails = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Email", Value = values[13] }
                            },
                            PhoneNumbers = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "Phone", Value = values[14] }
                            },
                            URLs = new List<LabeledValue<string>>
                            {
                                new LabeledValue<string> { Label = "URL", Value = values[15] }
                            }
                        }
                    };

                    persons.Add(person);
                }
            }

            return persons;
        }
    }


    internal class vCardDeserializer
    {
        public vCard Deserialize(string content)
        {
            // split content into lines
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            // create new vCard object
            var vCard = new vCard();
            // iterate over lines
            foreach (var line in lines)
            {
                // split line into key and value
                var parts = line.Split(':');
                // get key and value
                var key = parts[0];
                var value = parts[1];
                // set value to vCard object
                switch (key)
                {
                    case "FN":
                        var names = value.Split(' ');
                        vCard.FirstName = names[0];
                        vCard.LastName = names[1];
                        break;
                    case "BDAY":
                        vCard.BirthDate = DateTime.Parse(value);
                        break;
                    case "ADR":
                        var addressParts = value.Split(';');
                        vCard.Address = new vCardAddress
                        {
                            Label = addressParts[0],
                            Street = addressParts[2],
                            City = addressParts[3],
                            PostalCode = addressParts[4],
                            Country = addressParts[5]
                        };
                        break;
                    case "TEL":
                        vCard.Phone = value;
                        break;
                    case "EMAIL":
                        vCard.Email = value;
                        break;
                    case "URL":
                        vCard.Url = value;
                        break;
                }
            }

            return vCard;
                      
            
        }
    }
}
