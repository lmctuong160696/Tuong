using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProWeb.Models;

using PagedList;
using PagedList.Mvc;

namespace ProWeb.Controllers
{
    public class _HomeController : Controller
    {
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
        // GET: _Home
        DataQL_MYPHAMDataContext data = new DataQL_MYPHAMDataContext();
        public ActionResult Index()
        {
           
            return View();
        }

        //Hiển thị danh mục
        public ActionResult Menu(string id)
        {
            var menu = data.P_MENU(id).ToList();
            return PartialView(menu);
        }
        //Hiển thị tất cả loại hàng
        public ActionResult Brand()
        {
            var list = from l in data.HANGSANXUATs select l;
            return PartialView(list);
        }
        //Hiển thị loại hàng theo danh mục
        public ActionResult Menu_Category(string id)
        {
            var list = (from l in data.LOAISPs where id == l.MADM select l).ToList();
            return PartialView(list);
        }
        
        //Hiển thị 3 sản phẩm có giá cao nhất
        public ActionResult Product_Price_Max()
        {
            var list = data.PROCDUCT_PRICE_MAX().Take(3).ToList();
            return PartialView(list);
        }
        //Hiển thị popup chi tiết sản phẩm
        //public ActionResult Product_Detail_Popup(string id)
        //{
        //    var product = from detail in data.SANPHAMs where id == detail.MASP select detail;
        //    return PartialView(product);
        //}
        //Hiển thị trang chi tiết sản phẩm 
        public ActionResult Product_Detail(string id)
        {
            var detail = from product in data.SANPHAMs where id == product.MASP select product;
            return View(detail);
        }
        
        //Hiển thị sản phẩm theo thương hiệu
        public ActionResult Category_Brand(string id,int ? page)
        {
            //số sản phẩm mỗi trang
            int pageSize = 5;
            //số trang
            int pageNum = (page ?? 1);
            var list = data.P_BRAND_PRODUCT(id).Where(a=>a.MAHANG==id).ToList();
            return View(list.ToPagedList(pageNum,pageSize));
        }
        //Hiển thị sản phẩm theo loại hàng
        public ActionResult Category_Product(string id,int ? page)
        {
           
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var list = from l in data.SANPHAMs where id == l.MALOAI select l;
            return View(list.ToPagedList(pageNum, pageSize));
        }
        [HttpGet]
        public ActionResult Login()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection frm)
        {
            var tendn = frm["tendn"];
            var mk = frm["matkhau"];
            var kttendn = data.KHACHHANGs.SingleOrDefault(x => x.TENDN == tendn && x.MATKHAU ==mk);
            if(kttendn!= null)
            {
                Session["kh"] = kttendn;
                return RedirectToAction("Index","_Home");
            }
            else
            {
                ViewBag.thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }

         }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection frm)
        {
            var tenkh = frm["tenkh"];
            var gioitinh = frm["gioitinh"];
            var ngaysinh = frm["ngaysinh"];
            var diachi = frm["diachi"];
            var dienthoai = frm["dienthoai"];
            var email = frm["email"];
            var tendn = frm["tendangnhap"];
            var mk = frm["matkhau"];
            var kttendn = data.KHACHHANGs.SingleOrDefault(x => x.TENDN == tendn);

            if(kttendn!=null)
            {
                ViewBag.tendangnhap = "Tên đăng nhập đã tồn tại!";
                return View();
            }
            else
            {
                KHACHHANG kh = new KHACHHANG();
                kh.MAKH = Capma_tudong("KH");
                kh.TENKH = tenkh;
                if(gioitinh == "Nam")
                {
                    kh.GIOITINH = true;
                }
                else
                {
                    kh.GIOITINH = false;
                }

                kh.NGAYSINH = Convert.ToDateTime(ngaysinh);
                kh.DIACHI = diachi;
                kh.DIENTHOAI = dienthoai;
                kh.EMAIL = email;
                kh.TENDN = tendn;
                kh.MATKHAU = mk;

                kh.ACTIVE = true;
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                Update_ma("KH");
                return RedirectToAction("Login","_Home");
            }
          
        }
        public ActionResult Label()
        {
            return PartialView();
        }
        public ActionResult Logout()
        {
            Session["kh"] = null;
            return RedirectToAction("Index", "_Home");
        }
        //==============================================
       

        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang == null)
            {

                lstGiohang = new List<GioHang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;


        }

        public ActionResult ThemGiohang(string id, String URL)
        {
            List<GioHang> lstGiohang = Laygiohang();
            //Kiem tra sp nay co trong list chua

            GioHang sanpham = lstGiohang.Find(n => n.masp == id);
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGiohang.Add(sanpham);
                return Redirect(URL);



            }
            else
            {
                sanpham.soluong++;
                return Redirect(URL);


            }


        }

        private int TongSoLuong()
        {

            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.soluong);

            }
            return iTongSoLuong;

        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.thanhtien);

            }
            return iTongTien;

        }

      
        public ActionResult XoaGiohang(string id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.masp == id);

            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.masp == id);
                return RedirectToAction("GioHang");


            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "_Home");

            }
            return RedirectToAction("GioHang");



        }
        
        public ActionResult CapnhatGiohang(string id, FormCollection f)
        {

            List<GioHang> lstGiohang = Laygiohang();

            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.masp == id);

            var soluongg = f["txtsoluong"];
            if (sanpham != null)
            {
                sanpham.soluong = int.Parse(soluongg);

            }
            return RedirectToAction("Giohang");
        }
        public ActionResult Giohang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "_Home");

            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);

        }
        public ActionResult ThongTinGioHang()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            List<GioHang> lstGiohang = Laygiohang();

            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView(lstGiohang);
        }
        
        public ActionResult DatHang()
        {
            if (Session["kh"] == null)
                return RedirectToAction("Login", "_Home");
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
           
        }

        //Lưu đơn đặt hàng (Lấy từ Session[kh])
        public ActionResult LuuDDH()
        {
            List<GioHang> lstGiohang = Laygiohang();
            DONHANG dh = new DONHANG();
            KHACHHANG kh = (KHACHHANG)Session["kh"];
            dh.MADH = Capma_tudong("DH");
            dh.NGAYLAP = DateTime.Now;
            dh.TONGTIEN = (int)TongTien();
            dh.TINHTRANG = false;
            dh.MAKH = kh.MAKH;
            data.DONHANGs.InsertOnSubmit(dh);
            data.SubmitChanges();
            foreach(var item in lstGiohang )
            {
                CTDH ct = new CTDH();
                ct.MADH = Capma_tudong("DH");
                ct.MASP = item.masp;
                ct.SOLUONG = item.soluong;
                data.CTDHs.InsertOnSubmit(ct);
                data.SubmitChanges();
            }
            Update_ma("DH");
            Session["GioHang"] = null;
            return RedirectToAction("DatThanhCong", "_Home");
           
        }

        //Lưu chi tiết đơn đặt hàng (Lấy từ List<GioHang> lstGiohang = Laygiohang();)


        public ActionResult DatThanhCong()
        {
            return View();
        }

       
        public ActionResult Google_map()
        {
            return View();
        }
    }
}