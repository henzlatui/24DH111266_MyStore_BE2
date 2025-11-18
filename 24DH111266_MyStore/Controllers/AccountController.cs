using _24DH111266_MyStore.Models;
using _24DH111266_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList.Mvc;
using PagedList;


namespace _24DH111266_MyStore.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account      
        // Khởi tạo đối tượng Context/Entities để tương tác với cơ sở dữ liệu
        private MyStoreEntities db = new MyStoreEntities();

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào (Data Annotations)
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    // Nếu tên đăng nhập đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model); // Trả về View với lỗi
                }

                // Nếu chưa tồn tại thì tạo bản ghi thông tin tài khoản trong bảng User
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password, // Lưu ý: Nên mã hóa mật khẩu trước khi lưu
                    UserRole = "Customer"
                };

                db.Users.Add(user);

                // và tạo bản ghi thông tin khách hàng trong bảng Customer
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };

                db.Customers.Add(customer);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi validation
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.PropertyName + ": " + x.ErrorMessage);

                    // Ghép các lỗi thành một chuỗi
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Gán thông báo lỗi chi tiết vào ViewBag hoặc Log để hiển thị/kiểm tra
                    ViewBag.ValidationErrors = fullErrorMessage; // hoặc Console.WriteLine(fullErrorMessage);

                    // Bạn có thể giữ lại đoạn throw new System.Exception(fullErrorMessage, ex);
                    // Hoặc xử lý lỗi thân thiện hơn tại đây (ví dụ: trả về View kèm thông báo lỗi)
                    ModelState.AddModelError("", "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin. Lỗi chi tiết: " + fullErrorMessage);
                }

                // Chuyển hướng về trang chủ sau khi đăng ký thành công
                return RedirectToAction("Index", "Home");
            }

            // Nếu dữ liệu không hợp lệ, trả về View với dữ liệu hiện tại để hiển thị lỗi
            return View(model);
        }
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng trong CSDL bằng Username và Password.
                // Đồng thời kiểm tra UserRole có phải là "Customer" không.
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username
                                                      && u.Password == model.Password
                                                      && u.UserRole == "Customer");

                if (user != null)
                {
                    // LƯU Ý: Nếu đã mã hóa mật khẩu, dòng so sánh phía trên phải được thay thế
                    // bằng hàm so sánh mật khẩu đã mã hóa.

                    // Lưu trạng thái đăng nhập vào session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Lưu thông tin xác thực người dùng vào cookie (Forms Authentication)
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    // Chuyển hướng về trang chủ sau khi đăng nhập thành công
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Thêm lỗi vào ModelState nếu tên đăng nhập hoặc mật khẩu không đúng
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            // Nếu dữ liệu không hợp lệ hoặc đăng nhập thất bại, trả về View
            return View(model);
        }
    }
}