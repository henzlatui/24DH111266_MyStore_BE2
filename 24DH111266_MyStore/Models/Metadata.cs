using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _24DH111266_MyStore.Models
{
    public class CategoryMetadata
    {
        [HiddenInput]
        public int CatID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string NameCate { get; set; }
    }

    public class UserMetadata
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 5)]
        [RegularExpression("\r\n^[a-zA-Z0-9]([._-](?![._-])|[a-zA-Z0-9]){3,18}[a-zA-Z0-9]$\r\n")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string UserRole { get; set; }
    }

    public class CustomerMetadata
    {

    }

    public class SupplierMetadata
    {

    }

    //metadata của file product.cs
    public class ProductMetadata
    {
        [Display(Name = "Mã Sản Phẩm")]
        public int ProductID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Phải Nhập Tên Sản Phẩm")]
        [Display(Name = "Tên Sản Phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Chủng Loại Sản Phẩm")]
        public int CatID { get; set; }
        [Display(Name = "Đường Dẫn Ảnh Sản Phẩm")]
        [DefaultValue("~/Content/images/default_img.jfif")]

        //[DisplayName("")]
        public string ProductImage { get; set; }

        [Display(Name = "Mô Tả Sản Phẩm")]
        public string NameDecription { get; set; }

        [DefaultValue(true)]
        public System.DateTime CreatedDate { get; set; }
    }
}