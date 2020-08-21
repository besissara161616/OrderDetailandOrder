using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderListNew.Models.SQLServer;


namespace OrderListNew.Controllers
{
    public class OrderDetailPrintController : Controller
    {
        private readonly GraphiczoneDBContext _graphiczoneDBContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderDetailPrintController(GraphiczoneDBContext graphiczoneDBContext, IWebHostEnvironment hostEnvironment)
        {
            _graphiczoneDBContext = graphiczoneDBContext;
            this._webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserUsername") == null)
            {
                return RedirectToAction("");
            }
            else
            {
                return View();
            }

        }
        
        public IActionResult CreateOrder()
        {
            var x = _graphiczoneDBContext.OrderDetailPrint.ToList();
            List<OrderDetailPrint> orderDetailPrints = _graphiczoneDBContext.OrderDetailPrint.ToList();
            ViewBag.typeprint = _graphiczoneDBContext.TypePrint.ToList();
            var print = _graphiczoneDBContext.Print.FirstOrDefault();
            //var search = _graphiczoneDBContext.Print.Where(x => x.PrintId == orderDetailPrint.PrintId).FirstOrDefault();
            //ViewBag.printprice = search.PrintPrice;
            return View(x);
        }

        public JsonResult getselectbyid(string id)
        {
            List<Print> list = new List<Print>();
            list = _graphiczoneDBContext.Print.Where(x => x.TypePrint.TypePrintId == id).ToList();
            list.Insert(0, new Print { Id = 0, PrintId = "P000", PrintName = "กรุณาเลือกประเภทวัสดุ" });
            return Json(new SelectList(list, "PrintId", "PrintName"));
        }


        public ActionResult ListOrder()
        {

            return View();
        }
        public IActionResult TestOrder()
        {
            var listorderdetail = _graphiczoneDBContext.OrderDetailPrint.ToList();
            return View(listorderdetail);
        }
  
        public IFormFile Uploadfile { get; set; }

        [HttpPost]
        public async Task<IActionResult> uploadAsync(OrderPrint orderPrint, OrderDetailPrint orderDetailPrint,Print print,TypePrint typePrint)
        {
            orderDetailPrint.PrintId = orderDetailPrint.PrintId.Trim();
            //var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
            //if (searchData != null)
            //{
            //searchData.OrPrintStatus = orderPrint.OrPrintStatus;
            //orderDetailPrint.OrPrintId = orderPrint.OrPrintId;
            //var sessionid = HttpContext.Session.GetString("UserUsername");
            //var getUserid = _graphiczoneDBContext.User.Where(x => x.UserUsername == sessionid).FirstOrDefault();
            //shipping.UserId = getUserid.UserId;
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(Uploadfile.FileName);
            string extension = Path.GetExtension(Uploadfile.FileName);

            if (extension == ".jpg" || extension == ".png" || extension == ".gif")
            {
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                orderDetailPrint.OrdPrintFile = "img/" + fileName;
                fileName = Path.Combine(wwwRootPath, "img", fileName);

                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await Uploadfile.CopyToAsync(fileStream);
                }
            }
            var printid = _graphiczoneDBContext.Print.Where(x => x.PrintId == orderDetailPrint.PrintId).FirstOrDefault();
            orderDetailPrint.OrdPrintPriceset = printid.PrintPrice;
            orderDetailPrint.OrdPrintName = printid.PrintName;
            var genId = _graphiczoneDBContext.OrderDetailPrint.Count();
            orderDetailPrint.OrdPrintId = DateTime.Now.ToString("yyyyMdd") + "00" + (genId + 1).ToString();
            _graphiczoneDBContext.OrderDetailPrint.Add(orderDetailPrint);
            _graphiczoneDBContext.SaveChanges();
            //var x = "คุณได้ทำการบันทึกการส่งมอบ รายการที่ : " + orderPrint.OrPrintId + "เรียบร้อยแล้ว";
            return RedirectToAction("CreateOrder");
            //}

            //return Json("ไม่สำเร็จ");
        }
        [HttpPost]
        [HttpPost]
        public JsonResult deleteshipping(OrderDetailPrint orderDetailPrint)
        {
            var searchData = _graphiczoneDBContext.OrderDetailPrint.Where(x => x.OrPrintId == orderDetailPrint.OrdPrintId).FirstOrDefault();
            if (searchData != null)
            {
                searchData.OrdPrintId = orderDetailPrint.OrdPrintId;
                _graphiczoneDBContext.Remove(orderDetailPrint);
                _graphiczoneDBContext.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        //[HttpPost]
        //public JsonResult AddOrder()
        //{
        //    int OrPrintId = 0;
        //    OrderPrint order = new OrderPrint()
        //    {
        //        OrPrintDate = DateTime.Now

        //    };
        //    _graphiczoneDBContext.OrderPrint.Add(order);
        //    _graphiczoneDBContext.SaveChanges();
        //    OrPrintId = order.OrPrintId;
        //    foreach(var item in _graphiczoneDBContext)
        //    {
        //        OrderDetailPrint objorderDetailPrint = new OrderDetailPrint();
        //        objorderDetailPrint.OrdPrintTotal = item.OrdPrintTotal;
        //        objorderDetailPrint.OrdPrintId = item.OrdPrintId;
        //        objorderDetailPrint.OrPrintId = item.OrPrintId;
        //    }
        //    return Json("บันทึกสำเร็จ", JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        ////public JsonResult deleteshipping(OrderPrint orderPrint, Shipping shipping)
        ////{
        ////    var searchData = _graphiczoneDBContext.OrderPrint.Where(x => x.OrPrintId == orderPrint.OrPrintId).FirstOrDefault();
        ////    if (searchData != null)
        ////    {
        ////        searchData.OrPrintStatus = orderPrint.OrPrintStatus;
        ////        _graphiczoneDBContext.Remove(shipping);
        ////        _graphiczoneDBContext.SaveChanges();
        ////        return Json(1);
        ////    }
        ////    else
        ////    {
        ////        return Json(0);
        ////    }
        ////}
    }
}


