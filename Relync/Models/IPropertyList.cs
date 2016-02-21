using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Web;

namespace Relync.Models
{
    public interface IPropertyList
    {
        IEnumerable<PropertyList> GetAllProperties();
        PropertyList GetProperty(string Id);
        PropertyList AddProperty(PropertyList item, IEnumerable<HttpPostedFileBase> files);
        bool RemoveProperty(string Id);
        bool UpdateProperty(string Id, PropertyList item);
        PropertyList SaveProperty(PropertyList item);
        MongoDatabase Getdb();
        Comment AddCmmnt(string Id,Comment cmt);

        void RemoveComment(string Id, ObjectId commentID);
        IEnumerable<Comment> GetComments(string Id, int skip, int limit, int totalComments);
    }
}