using System.Collections.Generic;
using MongoDB.Driver;
using Geburtstage.Models;
using MongoDB.Bson;

namespace Geburtstage.Services
{
    public class PersonService
    {
        private readonly IMongoCollection<Person> _personsCollection;

        public PersonService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PersonenDB");
            _personsCollection = database.GetCollection<Person>("persons");
        }

        public List<Person> Get() => _personsCollection.Find(person => true).ToList();
        public Person Get(ObjectId id) => _personsCollection.Find<Person>(person => person.Id == id).FirstOrDefault();
        public Person Create(Person person)
        {
            if (person.Id == null)
            {
                person.Id = ObjectId.GenerateNewId();
            }
            _personsCollection.InsertOne(person);
            return person;
        }
        public void Update(ObjectId id, Person personIn) => _personsCollection.ReplaceOne(person => person.Id == id, personIn);
        public void Remove(ObjectId id) => _personsCollection.DeleteOne(person => person.Id == id);
        public void Clear() => _personsCollection.DeleteMany(_ => true);
    }
}
 