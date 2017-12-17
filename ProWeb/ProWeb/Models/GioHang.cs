using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProWeb.Models;

namespace ProWeb.Models
{
    public class GioHang
    {
        DataQL_MYPHAMDataContext data = new DataQL_MYPHAMDataContext();
        public string masp { set; get; }
        public string tensp { set; get; }
        public int giasp { set; get; }
        public string loaisp { set; get; }
        public string thuonghieu { set; get; }
        public int soluong { set; get; }
        public int thanhtien { get { return soluong * giasp; } }
        public string hinhanh { set; get; }

       


        public GioHang(string id)
        {
            var sanpham = data.SANPHAMs.SingleOrDefault(x=>x.MASP==id);
            var loai = data.LOAISPs.SingleOrDefault(x=>x.MALOAI==sanpham.MALOAI);
            var thuonghieuu = data.HANGSANXUATs.SingleOrDefault(x=>x.MAHANG==sanpham.MAHANG);
            hinhanh = sanpham.HINHANH;
            masp = id;
            tensp = sanpham.TENSP;
            giasp = (int)sanpham.DONGIA;
            loaisp = loai.TENLOAI;
            thuonghieu = thuonghieuu.TENHANG;
            soluong = 1;
            
        }
    }
}