using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using Relync.Models;
using System.Collections;
using Newtonsoft.Json;

namespace Relync.Controllers
{
    
    public class HistoryController : Controller
    {
       
        public static readonly IPropertyList ppty = new PropertyListRepository();
        
        // GET: History
        public ActionResult Index(string Id)
        {


            return View(ppty.GetProperty(Id));
        }

        [HttpPost]
        public ActionResult Index(string Id, HistoryModel hm)
        {
            var h_p_detail = ppty.GetProperty(Id);
          var _hm = new BsonDocument().Add("pricehistory",hm.PriceHistory).
                Add("hdate",hm.HDate).
                Add("event", hm.Event).
                Add("source", hm.Source);
            if (h_p_detail.ToString().Contains("PriceHistory"))
            {
                h_p_detail.ToBsonDocument()["PriceHistory"].AsBsonArray.Add(BsonValue.Create(_hm));
            }
            else
            {
                h_p_detail.ToBsonDocument()["PriceHistory"] = new BsonArray().Add(BsonValue.Create(_hm));
            }

            // TODO: Add update logic here
           // ppty.SaveProperty(Id);
            ppty.SaveProperty(h_p_detail);
            return RedirectToAction("Index", new { Id = Id });
            
        }
        // GET: History/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: History/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: History/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
       

        // GET: History/Edit/5
        public ActionResult Edit()
        {
           
            return View();
        }

        // POST: History/Edit/5
        [HttpPost]
        public ActionResult Edit(string Id,HistoryModel _hm)
        {
            try
            {
                
                return RedirectToAction("Index", new { Id = Id });
            }
            catch
            {
                return View();
            }
        }
        

        // GET: History/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: History/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
       
    }

}
