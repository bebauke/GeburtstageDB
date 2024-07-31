using Geburtstage.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using System;

public class Person
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string BirthName { get; set; }
    public string Prefix { get; set; }
    public string Suffix { get; set; }
    public string PlaceOfBirth { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PlaceOfDeath { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public Contact Contact { get; set; } = new Contact(); // Direkte Instanz von Contact
    public List<Relationship> Relationships { get; set; } = new List<Relationship>();

    public string FullName => $"{Prefix} {FirstName} {LastName} {Suffix}".Trim();

    public int Age
    {
        get
        {
            DateTime endDate = DateOfDeath ?? DateTime.Today;
            int age = endDate.Year - DateOfBirth.Year;
            if (endDate < DateOfBirth.AddYears(age)) age--;
            return age;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Person otherPerson)
        {
            return Id == otherPerson.Id;
        }
        return false;
    }
}
