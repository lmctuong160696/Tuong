using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProWeb.Models;
using System.IO;

namespace ProWeb.Controllers
{
    public class AdminSiteController : Controller
    {
        
        [HttpGet]
        public ActionResult Login()
        {
            Session["admin"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var tendn = form["tendn"];
            var matkhau = form["matkhau"];

            var kiemtra = data.ADMINs.SingleOrDefault(x => x.TENDN == tendn && x.MATKHAU == matkhau);
            if(kiemtra != null)
            {
                Session["admin"] = kiemtra;
                return RedirectToAction("Sanpham", "AdminSite");
            }
            else
            {
                ViewBag.thongbao = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }
        }
        DataQL_MYPHAMDataContext data = new DataQL_MYPHAMDataContext();
        public string Capma_tudong(string makitu)
        {
            int dem = 10; // Tổng chiều dài của mã.
            var tam = data.CAPMA_TUDONGs.SingleOrDefault(x => x.MAKITU == makitu); //  Tìm trường dữ liệu theo mã kí tự.
            int leng_ma = tam.MAKISO.ToString().Length; // Lấy chiều dài của mã kí số.
            int leng = dem - 2 - leng_ma; // leng là chiều dài của số "0".
            string ma_chinh = makitu;
            for (int i = 1; i < leng; i++)
            {
                ma_chinh += "0";
            }
            ma_chinh = ma_chinh.Trim() + tam.MAKISO.ToString();
            return ma_chinh;
        } //================================ Hàm cấp mã tự động.
        public void Update_ma(string makitu)
        {
            CAPMA_TUDONG capma = data.CAPMA_TUDONGs.SingleOrDefault(x => x.MAKITU == makitu);
            capma.MAKISO = capma.MAKISO + 1; // Mã kí số cộng thêm 1.
            data.SubmitChanges(); //Lưu lại
        }  //================================== Hàm update mã sau khi cấp.
        //
        // GET: /AdminSite/
        public ActionResult Index()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        public ActionResult Trangchu()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        public ActionResult Sanpham()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var sanpham = from sp in data.XEM_SANPHAMs where sp.ACTIVE == true select sp;
            return View(sanpham);
        }
        public ActionResult Xoasanpham(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var sanpham = data.SANPHAMs.SingleOrDefault(x => x.MASP == id);
            sanpham.ACTIVE = false;
            data.SubmitChanges();
            return RedirectToAction("Sanpham", "AdminSite");

        }
        public ActionResult Cbb_thuonghieu(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var thuonghieu = from h in data.HANGSANXUATs where h.ACTIVE == true && h.MAHANG!=id select h;
            return PartialView(thuonghieu);
        }
        public ActionResult Cbb_loai(string id)
        {
            var loai = from l in data.COMBOBOX_LOAIs where l.MALOAI!=id select l;
            return PartialView(loai);
        }
        public ActionResult Cbb_khuyenmai(string id)
        {
            var khuyenmai = from km in data.KHUYENMAIs where km.ACTIVE == true && km.MAKM!=id select km;
            return View(khuyenmai);
        }
        [HttpGet]
        public ActionResult Themsanpham()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themsanpham(FormCollection form, HttpPostedFileBase nhinhanh)
        {
            var tensanpham = form["tensanpham"];
            var loaisanpham = form["loaisanpham"];
            var thuonghieu = form["thuonghieu"];
            var giasanpham = form["giasanpham"];
            var khuyenmai = form["khuyenmai"];
            var mota = form["mota"];

            if (nhinhanh == null)
            {                            
                ViewBag.Thongbaohinhanh = "Chú ý: Vui lòng chọn hình ảnh!";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(nhinhanh.FileName);
                    var path = Path.Combine(Server.MapPath("/Content/web/img"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Thông báo: Hình ảnh đã tồn tại!";
                    }
                    else
                    {
                        SANPHAM sp = new SANPHAM();
                        sp.MASP = Capma_tudong("SP");
                        sp.TENSP = tensanpham;
                        sp.MALOAI = loaisanpham;
                        sp.MAHANG = thuonghieu;
                        sp.DONGIA = int.Parse(giasanpham);
                        sp.MAKM = khuyenmai;
                        sp.MOTA = mota;
                        sp.ACTIVE = true;
                        sp.TINHTRANG = true;
                        sp.HINHANH = fileName;
                        data.SANPHAMs.InsertOnSubmit(sp);
                        data.SubmitChanges();
                        Update_ma("SP");
                        nhinhanh.SaveAs(path);
                        ViewBag.Thongbaothanhcong = "Thêm sản phẩm thành công!";
                    }
                }
            }          
            return View();
        }
        public ActionResult CTSP(string id)
        {
            var sanpham = data.XEM_SANPHAMs.SingleOrDefault(x => x.MASP == id);
            return View(sanpham);
        }
        [HttpGet]
        public ActionResult Suasanpham(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var sanpham = data.XEM_SANPHAMs.SingleOrDefault(x => x.MASP == id && x.ACTIVE == true);
            return View(sanpham);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasanpham(string id,FormCollection form, HttpPostedFileBase nhinhanh)
        {
            var sp= data.XEM_SANPHAMs.SingleOrDefault(x => x.MASP == id);
            var tensanpham = form["tensanpham"];
            var loaisanpham = form["loaisanpham"];
            var thuonghieu = form["thuonghieu"];
            var giasanpham = form["giasanpham"];
            var khuyenmai = form["khuyenmai"];
            var mota = form["mota"];
            var sp2 = data.SANPHAMs.SingleOrDefault(x => x.MASP == id);
            if (nhinhanh == null)
            {
               
                
                sp2.TENSP = tensanpham;
                sp2.MALOAI = loaisanpham;
                sp2.MAHANG = thuonghieu;
                sp2.DONGIA = int.Parse(giasanpham);
                sp2.MAKM = khuyenmai;
                sp2.MOTA = mota;
                sp2.ACTIVE = true;
                sp2.TINHTRANG = true;              
             
                data.SubmitChanges();
                   
                ViewBag.Thongbaothanhcong = "Cập nhập thành công!";
                var sp1 = data.XEM_SANPHAMs.SingleOrDefault(x => x.MASP == id);
                return View(sp1);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(nhinhanh.FileName);
                    var path = Path.Combine(Server.MapPath("/Assets/img"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Thông báo: Hình ảnh đã tồn tại!";
                    }
                    else
                    {                       
                        sp2.TENSP = tensanpham;
                        sp2.MALOAI = loaisanpham;
                        sp2.MAHANG = thuonghieu;
                        sp2.DONGIA = int.Parse(giasanpham);
                        sp2.MAKM = khuyenmai;
                        sp2.MOTA = mota;
                        sp2.ACTIVE = true;
                        sp2.TINHTRANG = true;
                        sp2.HINHANH = fileName;                       
                        data.SubmitChanges();             
                        nhinhanh.SaveAs(path);
                        ViewBag.Thongbaothanhcong = "Cập nhật thành công!";
                    }
                }
            }
            var sp3 = data.XEM_SANPHAMs.SingleOrDefault(x => x.MASP == id);
            return View(sp3);
            
        }
        //==============================================================
        public ActionResult Danhmuc()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var danhmuc = from dm in data.DANHMUCs where dm.ACTIVE == true select dm;
            return View(danhmuc);
        }
        public ActionResult Xoadanhmuc(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var dm = data.DANHMUCs.SingleOrDefault(x => x.MADM == id);
            dm.ACTIVE = false;
            data.SubmitChanges();
            return RedirectToAction("Danhmuc", "AdminSite");

        }
        [HttpGet]
        public ActionResult Themdanhmuc()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        [HttpPost]
        public ActionResult Themdanhmuc(FormCollection form)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var tendanhmuc = form["tendanhmuc"];
            var kiemtratontai = data.DANHMUCs.SingleOrDefault(x => x.TENDM == tendanhmuc);
            if(kiemtratontai != null)
            {
                ViewBag.tendanhmuc = "***Tên danh mục đã tồn tại!";
                return View();
            }
            else
            {
                DANHMUC danhmuc = new DANHMUC();
                danhmuc.MADM = Capma_tudong("DM");
                danhmuc.TENDM = tendanhmuc;
                danhmuc.ACTIVE = true;
                data.DANHMUCs.InsertOnSubmit(danhmuc);
                data.SubmitChanges();
                ViewBag.thongbao = "Thêm thành công!";
                Update_ma("DM");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Capnhatdanhmuc(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var danhmuc = data.DANHMUCs.SingleOrDefault(x => x.MADM == id);
            return View(danhmuc);
        }
        [HttpPost]
        public ActionResult Capnhatdanhmuc(string id, FormCollection form)
        {
            var danhmuc = data.DANHMUCs.SingleOrDefault(x => x.MADM == id);
            var tendanhmuc = form["tendanhmuc"];
            var danhmuc_kiemtra = data.DANHMUCs.SingleOrDefault(x => x.TENDM == tendanhmuc && x.MADM != id);
            if(danhmuc_kiemtra != null)
            {
                ViewBag.tendanhmuc = "***Tên danh mục đã tồn tại!";
                return View(danhmuc);
            }
            else
            {
                danhmuc.TENDM = tendanhmuc;
                data.SubmitChanges();
                ViewBag.thongbao = "***Cập nhật thành công!";
            }
            var danhmuc1 = data.DANHMUCs.SingleOrDefault(x => x.MADM == id);
            return View(danhmuc1);
        }
        //==============================================================
        public ActionResult Combobox_danhmuc(string id)
        {
            var danhmuc = from dm in data.DANHMUCs where dm.ACTIVE == true && dm.MADM != id select dm;
            return PartialView(danhmuc);
        }
        public ActionResult Loaisanpham()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var loaisp = from l in data.XEM_LOAISPs where l.ACTIVE == true select l;
            return View(loaisp);
        }
        public ActionResult Xoaloai(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var loai = data.LOAISPs.SingleOrDefault(x => x.MALOAI == id);
            loai.ACTIVE = false;
            data.SubmitChanges();
            return RedirectToAction("Loaisanpham", "AdminSite");
        }
        [HttpGet]
        public ActionResult Themloai()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        [HttpPost]
        public ActionResult Themloai(FormCollection form)
        {
            var tenloai = form["tenloai"];
            var danhmuc = form["danhmuc"];
            var loai = data.LOAISPs.SingleOrDefault(x => x.TENLOAI == tenloai);
            if(loai != null)
            {
                ViewBag.tenloai = "***Tên loại đã tồn tại!";
                return View();
            }
            else
            {
                LOAISP l = new LOAISP();
                l.MALOAI = Capma_tudong("ML");
                l.TENLOAI = tenloai;
                l.MADM = danhmuc;
                l.ACTIVE = true;
                data.LOAISPs.InsertOnSubmit(l);
                data.SubmitChanges();
                Update_ma("ML");
                ViewBag.thongbao = "Thêm mới loại thành công!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Capnhatloai(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var loai = data.XEM_LOAISPs.SingleOrDefault(x => x.MALOAI == id);
            return View(loai);
        }
        [HttpPost]
        public ActionResult Capnhatloai(string id, FormCollection form)
        {
            var loai = data.LOAISPs.SingleOrDefault(x => x.MALOAI == id);
            var tenloai = form["tenloai"];
            var danhmuc = form["danhmuc"];
            var kiemtraloai = data.LOAISPs.SingleOrDefault(x => x.TENLOAI == tenloai && x.MALOAI != id);
            if(kiemtraloai != null)
            {
                ViewBag.tenloai = "***Tên loại đã tồn tại!";
                var loai1 = data.XEM_LOAISPs.SingleOrDefault(x => x.MALOAI == id);
                return View(loai1);
            }
            else
            {
                loai.TENLOAI = tenloai;
                loai.MADM = danhmuc;
                data.SubmitChanges();
                ViewBag.thongbao = "***Cập nhật thành công!";
            }
             var loai2 = data.XEM_LOAISPs.SingleOrDefault(x => x.MALOAI == id);
             return View(loai2);
        }
        //=================================================================
        public ActionResult Hangsanpham()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var hang = from h in data.HANGSANXUATs where h.ACTIVE == true select h;
            return View(hang);
        }
        [HttpGet]
        public ActionResult Themhang()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        [HttpPost]
        public ActionResult Themhang(FormCollection form)
        {
            var tenthuonghieu = form["tenthuonghieu"];
            var kiemtra = data.HANGSANXUATs.SingleOrDefault(x => x.TENHANG == tenthuonghieu);
            if(kiemtra != null)
            {
                ViewBag.tenthuonghieu = "***Tên thương hiệu đã tồn tại!";
                return View();
            }
            else
            {
                HANGSANXUAT h = new HANGSANXUAT();
                h.MAHANG = Capma_tudong("MH");
                h.TENHANG = tenthuonghieu;
                h.ACTIVE = true;
                data.HANGSANXUATs.InsertOnSubmit(h);
                data.SubmitChanges();
                Update_ma("MH");
                ViewBag.thongbao = "***Thêm thành công!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Capnhathang(string id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var hang = data.HANGSANXUATs.SingleOrDefault(x => x.MAHANG == id);
            return View(hang);
        }
        [HttpPost]
        public ActionResult Capnhathang(string id, FormCollection form)
        {
            var hang = data.HANGSANXUATs.SingleOrDefault(x => x.MAHANG == id);
            var tenhang = form["tenthuonghieu"];
            var kiemtra = data.HANGSANXUATs.SingleOrDefault(x => x.TENHANG == tenhang && x.MAHANG != id);
            if(kiemtra != null)
            {
                ViewBag.tenthuonghieu = "***Tên thương hiệu đã tồn tại!";
                return View(hang);
            }
            else
            {
                hang.TENHANG = tenhang;
                data.SubmitChanges();
                ViewBag.thongbao = "***Cập nhật thương hiệu thành công!";
            }
            var hang1 = data.HANGSANXUATs.SingleOrDefault(x => x.MAHANG == id);
            return View(hang1);
        }
        //==================================================================
        public ActionResult Khuyenmai()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            var khuyenmai = from km in data.KHUYENMAIs where km.ACTIVE == true select km;
            return View(khuyenmai);
        }
        [HttpGet]
        public ActionResult Themkhuyenmai()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Login", "AdminSite");
            return View();
        }
        [HttpPost]
        public ActionResult Themkhuyenmai(FormCollection form)
        {
            var giatrikm = form["giatrikm"];
            var kiemtra = data.KHUYENMAIs.SingleOrDefault(x => x.GIATRI == int.Parse(giatrikm));
            if(kiemtra != null)
            {
                ViewBag.khuyenmai = "***Giá trị khuyến mại đã tồn tại!";
                return View();
            }
            else
            {
                KHUYENMAI km = new KHUYENMAI();
                km.MAKM = Capma_tudong("KM");
                km.GIATRI = int.Parse(giatrikm);
                km.ACTIVE = true;
                data.KHUYENMAIs.InsertOnSubmit(km);
                data.SubmitChanges();
                ViewBag.thongbao = "***Thêm thành công!";
                Update_ma("KM");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Capnhatkhuyenmai(string id)
        {
            var khuyenmai = data.KHUYENMAIs.SingleOrDefault(x => x.MAKM == id);
            return View(khuyenmai);
        }
        [HttpPost]
        public ActionResult Capnhatkhuyenmai(string id , FormCollection form)
        {
            var khuyenmai = data.KHUYENMAIs.SingleOrDefault(x => x.MAKM == id);
            var giatrikm = form["giatrikm"];
            var kiemtra = data.KHUYENMAIs.SingleOrDefault(x => x.GIATRI == int.Parse(giatrikm) && x.MAKM != id);
            if(kiemtra != null)
            {
                ViewBag.khuyenmai ="***Giá trị khuyến mại đã tồn tại!";
                return View(khuyenmai);
            }
            else
            {
                khuyenmai.GIATRI = int.Parse(giatrikm);
                data.SubmitChanges();
                ViewBag.thongbao = "***Cập nhật thành công!";
            }
            var khuyenmai1 = data.KHUYENMAIs.SingleOrDefault(x => x.MAKM == id);
            return View(khuyenmai1);
        }  
        //==================================================================
        public ActionResult Khachhang()
        {
            var khachhang = from kh in data.KHACHHANGs where kh.ACTIVE == true select kh;
            return View(khachhang);
        }
        public ActionResult Dondathang()
        {
            var item = from i in data.XEM_DONDATHANGs select i;
        
            return View(item);
        }
        public ActionResult Chitietdonhang(string id)
        {
            var item = from i in data.XEM_CTDHs where i.MADH == id select i; 
            return View(item);
        }
	}
}