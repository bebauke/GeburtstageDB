using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Geburtstage.Models
{
    public class Relationship
    {
        public ObjectId Id { get; set; }
        public ObjectId PersonId1 { get; set; }
        public ObjectId PersonId2 { get; set; }
        public string Type { get; set; }
    }

}
