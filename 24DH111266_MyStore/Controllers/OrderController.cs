using _24DH111266_MyStore.Models;
using _24DH111266_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace _24DH111266_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        //GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //Kiểm tra giỏ hàng trong session
            //nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as Cart;
            if (cart == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //xác thực người dùng đã đăng nhập chưa, neeus chưa thì chuyển hướng tới trang đăng nhập
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Lấy thông tin khách hàng từ CSDL, nếu chưa thì chuyển hướng tới trang Đăng nhập
            //Nếu có rồi thì lấy địa chỉ của khách hàng và gán vào ShippingAddress của CheckoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null) { return RedirectToAction("Login", "Account"); }

            var model = new CheckoutVM //Tạo dữ liệu hiển thị cho Checkout
            {
                CartItems = cart.Items.ToList(), // Lấy danh sách sản phẩm trong giỏ hàng
                TotalAmount = cart.Items.Sum(item => item.TotalPrice), //Tổng giá trị của các mặt hàng trong giỏ
                OrderDate = DateTime.Now, //Mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress, //Lấy địa chỉ mặc định từ bảng Customer
                Username = customer.Username //Lấy tên đăng nhập từ bảng Customer
            };
            return View(model);
        }

        //POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as Cart;
                //Nếu giỏ hàng rỗng sẽ điều hướng tới trang Home
                if (cart == null) { return RedirectToAction("Index", "Home"); }

                //Nếu người dùng chưa đăng nhập thì sẽ điều hướng tới trang Login
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null) { return RedirectToAction("Login", "Account"); }

                //Nếu khách hàng không khớp với tên đăng nhập, sẽ điều hướng tới trang login
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }

                //Nếu người dùng chọn thanh toán = paypal, sẽ điều hướng tới trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {
                    return RedirectToAction("PaymentWithPaypal", "Paypal", model);
                }

                //Thiết lập paymentstatus dựa trên paymentmethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }

                //Tạo đơn hàng và chi tiết đơn hàng liên quan
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    DeliveryMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetails = cart.Items.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                    }).ToList()
                };
                //Lưu đơn hàng vào CSDL
                db.Orders.Add(order);
                db.SaveChanges();
                //Xóa giỏ hàng sau khi đặt hàng thành công
                Session["Cart"] = null;
                //Điều hướng tới trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }

        public ActionResult OrderSuccess(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = db.Orders
                          .Include("OrderDetails.Product")
                          .SingleOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        [Authorize]
        public ActionResult OrderHistory()
        {
            // 1. Lấy username của người dùng đang đăng nhập
            var tenDangNhap = User.Identity.Name;

            // 2. Tìm CustomerID từ username
            var customer = db.Customers.SingleOrDefault(c => c.Username == tenDangNhap);

            // 3. Kiểm tra xem customer có tồn tại không
            if (customer == null)
            {
                // Nếu không tìm thấy thông tin khách hàng, có thể đưa về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // 4. Lấy tất cả đơn hàng của khách hàng này
            // Sắp xếp theo ngày mới nhất lên trên
            var orders = db.Orders
                           .Where(o => o.CustomerID == customer.CustomerID)
                           .OrderByDescending(o => o.OrderDate)
                           .ToList(); // Lấy danh sách

            // 5. Gửi danh sách đơn hàng này tới View
            return View(orders);
        }
    }
}