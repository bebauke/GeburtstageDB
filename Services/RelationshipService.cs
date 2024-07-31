using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Geburtstage.Models;
using System;

namespace Geburtstage.Services
{
    public class RelationshipService
    {
        private readonly IMongoCollection<Relationship> _relationshipsCollection;

        public RelationshipService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PersonenDB");
            _relationshipsCollection = database.GetCollection<Relationship>("relationships");
        }

        public List<Relationship> Get() => _relationshipsCollection.Find(relationship => true).ToList();

        public Relationship Get(ObjectId id) => _relationshipsCollection.Find<Relationship>(relationship => relationship.Id == id).FirstOrDefault();

        public Relationship Create(Relationship relationship)
        {
            try
            {
                if (relationship.Id == ObjectId.Empty)
                {
                    relationship.Id = ObjectId.GenerateNewId();
                }
                _relationshipsCollection.InsertOne(relationship);
                return relationship;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Fehler beim Erstellen der Beziehung: {ex.Message}");
                return null;
            }
        }

        public void Update(ObjectId id, Relationship relationshipIn)
        {
            try
            {
                _relationshipsCollection.ReplaceOne(relationship => relationship.Id == id, relationshipIn);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Fehler beim Aktualisieren der Beziehung: {ex.Message}");
            }
        }

        public void Remove(ObjectId id)
        {
            try
            {
                _relationshipsCollection.DeleteOne(relationship => relationship.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Fehler beim Entfernen der Beziehung: {ex.Message}");
            }
        }

        public void Clear() => _relationshipsCollection.DeleteMany(_ => true);

        //public void RemoveByPersonId(ObjectId id)
        //{
        //    try
        //    {
        //        _relationshipsCollection.DeleteMany(relationship => relationship.PersonId1 == id || relationship.PersonId2 == id);
        //        Console.WriteLine($"Alle Beziehungen für PersonId {id} wurden entfernt.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine($"Fehler beim Entfernen der Beziehungen für PersonId {id}: {ex.Message}");
        //    }
        //}

        internal List<Relationship> GetByPersonId(ObjectId id)
        {
            return _relationshipsCollection.Find<Relationship>(relationship => relationship.PersonId1 == id || relationship.PersonId2 == id).ToList();
        }
    }
}
