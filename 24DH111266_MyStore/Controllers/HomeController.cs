using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using _24DH111266_MyStore.Models;
using _24DH111266_MyStore.Models.ViewModel;
using PagedList;

namespace _24DH111266_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            //Tìm kiếm sản phẩm dựa trên từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                        p.ProductDescription.Contains(searchTerm) ||
                                        p.Category.CategoryName.Contains(searchTerm));
            }

            //Đoạn code liên quan tới phân trang
            //Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = 6; //Số sản phẩm mỗi trang

            //Láy top 10 sản phẩm bán chạy nhất
            model.FeaturedProducts = db.Products
                                        .OrderByDescending(p => p.OrderDetails.Count())
                                        .Take(10)
                                        .ToList();

            //Lấy 20 sản phẩm và phân trang
            model.NewProducts = products
                        .OrderBy(p => p.OrderDetails.Count())
                        .Take(20)
                        .ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        // GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            // Lấy tất cả các sản phẩm cùng danh mục
            var products = db.Products
                .Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID)
                .AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; // Số sản phẩm mỗi trang

            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PVFeatureProduct()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult PVNewProduct()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}