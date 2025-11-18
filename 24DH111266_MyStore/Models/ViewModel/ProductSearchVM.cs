using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using PagedList;


namespace _24DH111266_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        //Tiêu chí để search theo tên, mô tả sp hoặc loai sp
        public string SearchTerm { get; set; }

        //Các tiêu chí để search theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        //Thứ tự sắp xếp
        public string SortOrder { get; set; }

        //Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } //Trang hiện tại
        public int PageSize { get; set; } = 10; //Số sản phẩm mỗi trang

        //Danh sách sản phẩm đã phân trang
        public PagedList.IPagedList<Product> Products { get; set; }
        //Danh sách sản phẩm thỏa điều kiện tìm kiếm
        //Public List<Product> Products {get; set;}
    }
}