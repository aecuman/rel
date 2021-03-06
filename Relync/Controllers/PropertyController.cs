﻿using MongoDB.Bson;
using Relync.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Relync.Controllers
{

    public class PropertyController : Controller
    {
        public static readonly IPropertyList _property = new PropertyListRepository();


        public ActionResult Index(string Typ, string locn, string px)
        {
            if (!string.IsNullOrEmpty(Typ) && !string.IsNullOrEmpty(locn) || !string.IsNullOrEmpty(px))
            {
                TempData["Locn"] = locn;
                TempData["Type"] = Typ;
                TempData["px"] = px;
                return RedirectToAction("Map");
            }
            return View();

        }
        public Size NewImgSz(Size imgSz, Size newSz)
        {
            Size finalSz;
            double tempVal;
            if (imgSz.Height > newSz.Height || imgSz.Width > newSz.Width)
            {
                if (imgSz.Height > imgSz.Width)
                {
                    tempVal = newSz.Height / (imgSz.Height * 1.0);
                }
                else
                {
                    tempVal = newSz.Width / (imgSz.Width * 1.0);
                }
                finalSz = new Size((int)(tempVal * imgSz.Width), (int)(tempVal * imgSz.Height));

            }
            else finalSz = imgSz;



            return finalSz;


        }
        private void SaveToFolder(Image img, string fileName, string exte, Size Newsize, string pathToSave)
        {
            Size ImgSize = NewImgSz(img.Size, Newsize);
            using (Image newImg = new Bitmap(img, ImgSize.Width, ImgSize.Height))
            {
                newImg.Save(Server.MapPath(pathToSave), img.RawFormat);
            }
        }
        public ActionResult Map(string typ)
        {
            if (!string.IsNullOrEmpty(typ))
            {
                try
                {
                    var _ppty = _property.GetAllProperties().Where(p => p.pptyType.Contains(typ) && p.Availability.Equals(true));


                    return View(_ppty.ToList());
                }

                catch
                {

                    return View(_property.GetAllProperties().AsEnumerable());
                }
            }
            else
            {
                return View(_property.GetAllProperties().AsEnumerable().Where(u => u.Availability == true));
            }
        }

        // GET: Property
        public ActionResult List()
        {
            var ent = new PropertyListRepository();
            // var immg = _property.Getdb().GridFS.FindAll();
            var model = _property.GetAllProperties().AsEnumerable();
            foreach (var ppt in model)
            {
                var editor = new PropertyList()
                {
                    Id = ppt.Id,
                    Availability = ppt.Availability
                };
                /// ent.AddProperty(editor, null);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult changeAvail(PropertyList model)
        {
            var selid = model.Id;
            var selctedppty = from x in _property.GetAllProperties()
                              where selid.Contains(x.Id)
                              select x;
            foreach (var pp in selctedppty)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", pp.Id, pp.Availability));
            }
            return RedirectToAction("List");
        }

        // GET: Property/Details/5
      [HttpGet]
        public ActionResult Details(string Id)
        {

            var p_detail = _property.GetProperty(Id);
            ViewBag.PostId = p_detail.Id;
            ViewBag.TotalComments = p_detail.TotalComments;
            ViewBag.LoadedComments = 5;

            return View(p_detail);
        }
        [HttpGet]
        public ActionResult Detailsale(string Id)
        {

            var p_detail = _property.GetProperty(Id);
            ViewBag.PostId = p_detail.Id;
            ViewBag.TotalComments = p_detail.TotalComments;
            ViewBag.LoadedComments = 5;

            return View(p_detail);
        }
        // GET: Property/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Property/Create
        [HttpPost]
        public ActionResult Create(PropertyList property, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase planLink, HttpPostedFileBase thumbpic)
        {

            try
            {
                if (files.Count() == 0 || files.FirstOrDefault() == null)
                {
                    ViewBag.error = "Please choose a file";
                }
                List<ImageGallery> Piclist = new List<ImageGallery>();
                foreach (var file in files)
                {
                    var model = new ImageGallery();
                    if (Piclist.Count == 0)
                    {
                        model.ID = "1";
                    }
                    else
                    {
                        model.ID = (Piclist.Count + 1).ToString();
                    }
                  
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    using (var img = Image.FromStream(file.InputStream))
                    {
                        var imgname = Guid.NewGuid().ToString();
                        model.ImagePath = string.Format("/Images/GalleryImages/{0}{1}", imgname, ext);
                        //Save Large Size
                        SaveToFolder(img, model.ID, ext, new Size(600, 400), model.ImagePath);
                        //Save thumb Size
                      
                    }
                    Piclist.Add(model);

                }
                var xt = Path.GetExtension(planLink.FileName).ToLower();
                var pat = string.Format("/Images/GalleryImages/Plans/{0}{1}", planLink.FileName, xt);
                
                using (var ig = Image.FromStream(planLink.InputStream))
                {
                    SaveToFolder(ig, planLink.FileName, xt, new Size(600, 400), pat);
                }
                var thumbname = Guid.NewGuid().ToString();
                var thmbxt = Path.GetExtension(thumbpic.FileName).ToLower();
                property.thumbpic = string.Format("/Images/GalleryImages/thumbs/{0}{1}", thumbname, thmbxt);
                using (var th = Image.FromStream(thumbpic.InputStream))
                {
                    SaveToFolder(th,thumbname,thmbxt,new Size(400,150),property.thumbpic);
                }
                property.planLink = pat;
                property.ImageList = Piclist;
                _property.AddProperty(property, files);

                // TODO: Add insert logic here



                return RedirectToAction("Map");
            }
            catch
            {
                return View(property);
            }
        }


        // GET: Property/Edit/5
        public ActionResult Edit(string Id)
        {
            return View(_property.GetProperty(Id));
        }
        public ActionResult Gallery(string Id)
        {
            var currentgal = _property.GetProperty(Id).ImageList;
            foreach (var pc in currentgal)
            {

            }

            return View(_property.GetProperty(Id));
        }

        // POST: Property/Edit/5
        [HttpPost]
        public ActionResult Edit(string Id, PropertyList item)
        {
            try
            {
                // TODO: Add update logic here
                _property.UpdateProperty(Id, item);
                return RedirectToAction("Map");
            }
            catch
            {
                return View();
            }
        }

        // GET: Property/Delete/5
        public ActionResult Delete(int? Id)
        {
            return View();
        }

        // POST: Property/Delete/5
        [HttpPost]
        public ActionResult Delete(string Id)
        {
            try
            {
                // TODO: Add delete logic here
                _property.RemoveProperty(Id);
                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AddComment(string Id, Comment cmt)
        {
            if (ModelState.IsValid)
            {
                var newComment = new Comment() {
                    commentID = cmt.commentID,
                    Name = cmt.Name,
                    Cmmnt = cmt.Cmmnt,
                    Date = cmt.Date

                };
                _property.AddCmmnt(Id, cmt);
                ViewBag.PostId = Id;
                return Json(

                    new
                    {
                        Result="ok",
                        CommentHtml=RenderPartialViewToString("Comment",newComment),
                        FormHtml=RenderPartialViewToString("AddComment", new Comment())
                    });
            }
            ViewBag.PostId = Id;
            return Json(
               new
               {
                   Result = "fail",
                   FormHtml = RenderPartialViewToString("AddComment", cmt)
               });

        }
        
        public ActionResult RemoveComment(string Id,ObjectId commentID)
        {
            
            _property.RemoveComment(Id, commentID);
            return new EmptyResult();

        }
        [HttpPost]
        public ActionResult CommentList(string Id,int skip,int limit,int totalComments)
        {
            ViewBag.TotalComments = totalComments;
            ViewBag.LoadedComments = skip + limit;
            return PartialView(_property.GetComments(Id, ViewBag.LoadedComments, limit, totalComments));
        }
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
