
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Relync.Models
{
   
    public class PropertyList
    {
        
        public PropertyList()
        {
            PriceHistory = new List<HistoryModel>();
            ImageList = new List<ImageGallery>();
            Commnt = new List<Comment>();
        }
      
        [BsonId]
        public string Id { get; set; }
        [BsonRequired]
        public string pptyType { get; set; }
        public string Category { get; set; }
   
        public string District { get; set; }
     
        public string Suburb { get; set; }

        public string Place { get; set; }

        public double Bedrooms { get; set; }

        public double Baths { get; set; }
        public decimal Price { get; set; }


      
        public double lat { get; set; }
        public double lon { get; set; }   

        public bool Availability { get; set; }
        [UIHint("WYSIWYG")]
        [AllowHtml]
        public string GDescription { get; set; }

        [UIHint("WYSIWYG")]
        [AllowHtml]
        public string Detail { get; set; }
        public IList<HistoryModel> PriceHistory { get; set; }
      

     
        public string ListedBy { get; set; }
        [DataType(DataType.DateTime)]
        
        public DateTime Date { get; set; }

        public string ContactName { get; set; }

        public string TypContact { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public string Contact { get; set; }
        public List<ImageGallery>ImageList { get; set; }
        public string planLink { get; set; }
        public string vidLink { get; set; }
        public string thumbpic { get; set; }
        public List<Comment> Commnt { get; set; }
        public int TotalComments { get; set; }


    }


   
    public class HistoryModel
    {
        [BsonId]
        public string id { get; set; }

        public DateTime HDate { get; set; }
    
        public int PriceHistory { get; set; }

        public string Event { get; set; }

        public string Source { get; set; }


    }
    public  class ImageGallery
    {
        
       public string ID { get; set; }      
        public string ImagePath { get; set; }
     
    }

    public class Comment
    {
        [BsonId]
        public ObjectId commentID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Cmmnt { get; set; }
        public DateTime Date { get; set; }
    }

}
