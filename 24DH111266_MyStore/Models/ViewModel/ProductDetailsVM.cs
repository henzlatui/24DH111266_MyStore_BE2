using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using PagedList;

namespace _24DH111266_MyStore.Models.ViewModel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;
        //Tính giá trị tạm thời
        public decimal estimatedValue => quantity * product.ProductPrice;

        //Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } //Trang hiện tại
        public int PageSize { get; set; } = 3; //Số sản phẩm mỗi tran

        //Danh sách 8 sản phẩm cùng danh mục
        public PagedList.IPagedList<Product> RelatedProducts { get; set; }

        //Danh sách 8 sản phẩm bán chạy nhất cùng danh mục
        public PagedList.IPagedList<Product> TopProducts { get; set; }
    }
}