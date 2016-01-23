using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Web;
using System;

namespace Relync.Models
{

    public class PropertyListRepository : IPropertyList
    {
        // YouTubeRequestSettings setting = new YouTubeRequestSettings("Relync", "AIzaSyAKHkHEx5ytSCJd5HbDpoR8udARGuES7XA") { Timeout = 999999999 };

        public PropertyListRepository()
            : this("")
        {
        }
        MongoServer _server;
        MongoDatabase _database;
        MongoCollection<PropertyList> _property;
        public PropertyListRepository(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection))
            {

               // connection = "mongodb://localhost:27017/propertydb";
                 connection = "mongodb://appharbor_xlnbpk5k:q4uttrsjb3oelhqtcgh6e7v9vg@ds039155.mongolab.com:39155/appharbor_xlnbpk5k";
            }
            MongoClient mongoClient = new MongoClient(connection);
           //  _server = mongoClient.GetDatabase("propertydb");
            // MongoDatabase db = mongoClient.GetDatabase("propertydb"); 
            //_database = mongoClient.GetServer().GetDatabase("propertydb");
            _database = mongoClient.GetServer().GetDatabase("appharbor_xlnbpk5k");
            _property = _database.GetCollection<PropertyList>("propertylist");
            // IndexKeysBuilder Key = IndexKeys.GeoSpatial("list");
            //  IndexOptionsBuilder options = IndexOptions.SetUnique(true).SetDropDups(true);
            //  _property.CreateIndex(Key, options);



        }
        public PropertyList AddProperty(PropertyList item, IEnumerable<HttpPostedFileBase> files)
        {


            item.Id = BsonObjectId.GenerateNewId().ToString();
            item.Contact = (_property.Count() + 1).ToString();
            item.Date = DateTime.Now;
           // Uploadvid(item.Contact, vid);
            _property.Insert(item);
            return item;
        }


        public IEnumerable<PropertyList> GetAllProperties()
        {



            return _property.FindAll();
        }

        public PropertyList GetProperty(string Id)
        {


            var query = Query.EQ("_id", Id);
            //IMongoQuery query = Query.EQ("_id", Id);
            var ppty = _property.Find(query).FirstOrDefault();
            return ppty;
        }
        public MongoDatabase Getdb()
        {
            return _database;
        }
        
        public bool RemoveProperty(string Id)
        {
            IMongoQuery query = Query.EQ("_id", Id);
            WriteConcernResult result = _property.Remove(query);
            return result.DocumentsAffected == 1;
        }

        public bool UpdateProperty(string Id, PropertyList item)
        {

            IMongoQuery query = Query.EQ("_id", Id);
            item.Date=DateTime.Now;
            IMongoUpdate update = Update
                .Replace(item);
            WriteConcernResult result = _property.Update(query, update);
            return result.UpdatedExisting;
        }
        public PropertyList SaveProperty(PropertyList item)
        {

            _property.Save(item);
            return item;
        }
      


        
       

    }

}