
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Relync.Models
{
    public class PropertyList
    {
        
        public PropertyList()
        {
            PriceHistory = new List<HistoryModel>();
            ImageList = new List<ImageGallery>();
        }
      
        [BsonId]
        public string Id { get; set; }
        [BsonRequired]
        public string pptyType { get; set; }
        public string Category { get; set; }
        [BsonElement("district")]
        public string District { get; set; }
        [BsonElement("suburb")]
        public string Suburb { get; set; }
        [BsonElement("place")]
        public string Place { get; set; }
        [BsonElement("bedrooms")]
        public double Bedrooms { get; set; }
        [BsonElement("baths")]
        public double Baths { get; set; }
        public string Price { get; set; }
        [BsonElement("area")]
        public string Area { get; set; }
      
        public double lat { get; set; }
        public double lon { get; set; }   
        [BsonElement("availability")]
        public bool Availability { get; set; }
        [BsonElement("generaldescription")]
        public string GDescription { get; set; }
        [BsonElement("facts")]
        public string Facts { get; set; }
        [BsonElement("rooms")]
        public string Rooms { get; set; }
        [BsonElement("construction")]
        public string Construction { get; set; }
        [BsonElement("other")]
        public string Other { get; set; }
        [BsonElement("PriceHistory")]
        public IList<HistoryModel> PriceHistory { get; set; }
        [BsonElement("nearby")]
        public string Nearby { get; set; }

        [BsonElement("listedby")]
        public string ListedBy { get; set; }
        [DataType(DataType.DateTime)]
        
        public DateTime Date { get; set; }
        [BsonElement("contactname")]
        public string ContactName { get; set; }
        [BsonElement("typcontact")]
        public string TypContact { get; set; }
        [BsonElement("phone")]
        public string Phone { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        public string Contact { get; set; }
        public List<ImageGallery>ImageList { get; set; }
        public string planLink { get; set; }
        public string vidLink { get; set; }


    }


   
    public class HistoryModel
    {
        [BsonId]
        public string id { get; set; }
        [BsonElement("hdate")]
        public DateTime HDate { get; set; }
        [BsonElement("pricehistory")]
        public int PriceHistory { get; set; }
        [BsonElement("event")]
        public string Event { get; set; }
        [BsonElement("source")]
        public string Source { get; set; }


    }
    public  class ImageGallery
    {
        
       public string ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string ThumbPath { get; set; }
    }



}
