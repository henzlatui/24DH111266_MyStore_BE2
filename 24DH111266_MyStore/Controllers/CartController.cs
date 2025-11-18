using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH111266_MyStore.Models;
using _24DH111266_MyStore.Models.ViewModel;
using PagedList;
using PagedList.Mvc;

namespace _24DH111266_MyStore.Controllers
{
    public class CartController: Controller
    {
        // GET: Cart
        private MyStoreEntities db = new MyStoreEntities();

        //Hàm lấy dịch vị giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        //Hiển thị giỏ hàng không gom nhóm theo danh mục
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        // Hiển thị giỏ hàng đã gom nhóm sản phẩm theo danh mục
        public ActionResult Index2(int ? page)
        {
            var cart = GetCartService().GetCart();
            var products = db.Products.ToList();
            var similarProducts = new List<Product>();

            if (cart.Items != null && cart.Items.Any())
            {
                similarProducts = products.Where(p => cart.Items.Any(ci => ci.Category == p.Category.CategoryName)
                && !cart.Items.Any(ci => ci.ProductID == p.ProductID)).ToList();
            }

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = cart.PageSize; // Số sản phẩm mỗi trang

            cart.SimilarProducts = similarProducts.OrderBy(p => p.ProductID).ToPagedList(pageNumber, pageSize);
            return View(cart);
        }

        //Thêm sản phẩm vào giỏ
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(product.ProductID, product.ProductImage,
                    product.ProductName, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }

        //Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCArt(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        //Làm trống giỏ hàng
        public ActionResult ClearCart()
        {
            var cartService = ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}