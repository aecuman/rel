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

                 connection = "mongodb://localhost:27017/propertydb";
               // connection = "mongodb://admin:relync101@ds019839.mlab.com:19839/propertydb";
            }
            MongoClient mongoClient = new MongoClient(connection);
           //  _server = mongoClient.GetDatabase("propertydb");
            // MongoDatabase db = mongoClient.GetDatabase("propertydb"); 
        _database = mongoClient.GetServer().GetDatabase("propertydb");
         // _database = mongoClient.GetServer().GetDatabase("appharbor_xlnbpk5k");
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
            _property.Save(item);
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
      public Comment AddCmmnt(string Id,Comment cmt)
        {
            //IMongoQuery query = Query.EQ("_id", Id);  
            cmt.commentID = ObjectId.GenerateNewId();          
            cmt.Date = DateTime.Now;
            _property.Update(Query.EQ("_id", Id), Update.PushWrapped("Commnt", cmt).Inc("TotalComments", 1));
            return cmt;
        }

        public void RemoveComment(string Id, ObjectId commentID)
        {

            _property.Update(Query.EQ("_id", Id),
               Update.Pull("Commnt", Query.EQ("_id", commentID))/*.Inc("TotalComments", -1)*/);
        }

        public IEnumerable<Comment> GetComments(string Id, int skip, int limit, int totalComments)
        {
            var newComments = GetTotalComments(Id) - totalComments;
            skip += newComments;
            var ppt = _property.Find(Query.EQ("_id", Id)).SetFields(Fields.Exclude("pptyType", "Category", "District", "Suburb", "Place", "Bedrooms", "Baths", "Price", "Area", "lat", "lon", "Availability", "GDescription", "Facts", "Rooms", "Construction", "Other", "PriceHistory", "Nearby", "ListedBy", "Date", "ContactName", "TypeContact", "Phone", "Email", "Contact", "ImageList", "planLink", "vidLink", "TotalComments").Slice("Commnt", -skip, limit)).Single();
            return ppt.Commnt.OrderByDescending(c => c.Date);
        }
        public int GetTotalComments(string Id)
        {
            var pty = _property.Find(Query.EQ("_id", Id)).SetFields(Fields.Include("TotalComments")).Single();
            return pty.TotalComments;
        }
    }

}