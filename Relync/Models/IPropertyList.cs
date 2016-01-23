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
     


    }
}