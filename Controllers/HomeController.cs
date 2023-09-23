using demo.Models;
using HiTech.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Net.Http.Headers;
using MessagePack;

namespace HiTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(Home user)
        {

            DataSet ds = user.allproduct();
            ViewBag.data = ds.Tables[0];
            DataSet dataset = user.NormalProduct();
            ViewBag.Normal = dataset.Tables[0];
            List<String> imggUrls = new List<String>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                imggUrls.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
            }
            ViewBag.ProductImages = imggUrls;
            List<String> img = new List<String>();
            foreach (DataRow dr in dataset.Tables[0].Rows)
            {
                img.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
            }
            ViewBag.Productimaage = img;
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (TempData.Peek("id") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(Home login)
        {
            string status = "true";
            DataSet ds = login.login(login.email, login.password, status);
            ViewBag.data = ds.Tables[0];
            foreach (System.Data.DataRow row in ViewBag.data.Rows)
            {

                TempData["id"] = row["id"].ToString();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet]

        public IActionResult Register()
        {

            if (TempData.Peek("id") != null)
            {

                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Register(Home rg)
        {
            string name = rg.name;
            string email = rg.email;
            string password = rg.password;
            int total = 2;
            int num = 0;
            int totalBid = 5;
            try
            {
                int dataAdd = rg.register(name, email, password);
                int id = Convert.ToInt32(TempData["id"]);
                if (dataAdd != 0)
                {
                    DataSet user = rg.SelectLastRecord();
                    ViewBag.id = user.Tables[0];
                }
                foreach (System.Data.DataRow row in ViewBag.id.Rows)
                {
                    id = Convert.ToInt32(row["id"]);
                }
                rg.upload_product(id, total, num, num, totalBid);
            }
            catch
            {
                string message = "Email is alredy taken";
                ViewBag.errorMessage = message;
                return RedirectToAction("ErrorMessageRegister");
            }
            return RedirectToAction("Login");
        }
        public IActionResult ErrorMessageRegister()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Bid(Home user, int id, int a = 0)
        {
            int user_id;
            if (TempData.Peek("id") != null)
            {

                DataSet dataset = user.select_product(id);
                ViewBag.SelectedProduct = dataset.Tables[0];
                List<String> imgurl = new List<String>();
                foreach (DataRow dr in dataset.Tables[0].Rows)
                {
                    imgurl.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
                }
                ViewBag.product_img = imgurl;
                foreach (System.Data.DataRow row in ViewBag.SelectedProduct.Rows)
                {
                    DataSet bid = user.winnerBid(id);
                    ViewBag.BidData = bid.Tables[0];
                    int counter = 0;
                    foreach (System.Data.DataRow dr in ViewBag.BidData.Rows)
                    {
                        counter++;
                    }
                    if (counter == 2)
                    {
                        DataSet win = user.winner(id);
                        ViewBag.winnr = win.Tables[0];
                        foreach (System.Data.DataRow row1 in ViewBag.winnr.Rows)
                        {
                            int winerId = Convert.ToInt32(row1["bid_id"]);
                            int userid = Convert.ToInt32(row1["user_id"]);
                            int productid = Convert.ToInt32(row1["product_id"]);
                            var bidalue = row1["bid_value"].ToString();
                            int add = user.updateWinner(winerId);
                            user.updateProductPrice(bidalue,productid);
                            user.addtocart(userid, productid);
                            user.updateAuction(id);
                        }
                    }
                    user_id = Convert.ToInt32(row["user_id"]);
                    DataSet ds = user.owner(user_id);
                    ViewBag.owner = ds.Tables[0];
                }
                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult Bid(Home user, int id)
        {
            int user_id = int.Parse((string)TempData.Peek("id"));
            DataSet ds = user.SlectNumProduct(user_id);
            ViewBag.remainingBid = ds.Tables[0];
            foreach (System.Data.DataRow row in ViewBag.remainingBid.Rows)
            {
                if (Convert.ToInt32(row["num_bid"]) < Convert.ToInt32(row["total_bid"]))
                {
                    int RemainingBid = Convert.ToInt32(row["num_bid"]);

                    int number = user.SubmitBid(user_id, id, user.bid_value, user.bid_time, user.bidder_name);
                    if (number > 0)
                    {
                        int remain = RemainingBid + 1;
                        user.updateRemainingBid(remain, user_id);
                    }
                    else
                    {
                        return RedirectToAction("ErrorMessageBid");
                    }

                }
            }
            return RedirectToAction("Bid");

        }
        public IActionResult ErrorMessageBid()
        {
            return View();
        }
        public IActionResult ErrorMessageProduct()
        {
            return View();
        }
        public IActionResult PaymentSuccess()
        {
            return View();
        }
        public IActionResult PaymentFailure()
        {
            return View();
        }
        public IActionResult AboutUs(Home user)
        {
            DataSet ds = user.allTeam();
            ViewBag.data = ds.Tables[0];
            List<String> imggUrls = new List<String>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                imggUrls.Add(Url.Content("~/Products/") + dr["image"].ToString());
            }
            ViewBag.Teamimg = imggUrls;
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult Blog(Home user)
        {
            DataSet ds = user.allblog();
            ViewBag.data = ds.Tables[0];
            List<String> imggUrls = new List<String>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                imggUrls.Add(Url.Content("~/Products/") + dr["img"].ToString());
            }
            ViewBag.Blogimg = imggUrls;
            return View();
        }
        public IActionResult Buy(Home user)
        {
            DataSet ds = user.allproduct();
            ViewBag.data = ds.Tables[0];
            DataSet dataset = user.NormalProduct();
            ViewBag.Normal = dataset.Tables[0];
            List<String> imggUrls = new List<String>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                imggUrls.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
            }
            ViewBag.ProductImages = imggUrls;
            List<String> img = new List<String>();
            foreach (DataRow dr in dataset.Tables[0].Rows)
            {
                img.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
            }
            ViewBag.Productimaage = img;
            return View();
        }
        [HttpGet]
        public IActionResult Contact(Home user, int a = 0)
        {
            if (TempData.Peek("id") != null)
            {
                //int id = int.Parse((string)TempData.Peek("id"));

                //DataSet ds = user.contactus_page(id);
                //ViewBag.data = ds.Tables[0];
                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult Contact(Home add)
        {
            int id = int.Parse((string)TempData.Peek("id"));
            add.contactUs(id, add.f_name, add.l_name, add.email, add.mobile, add.message);
            return RedirectToAction("ReplyPage");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Product(Home user)

        {
            if (TempData.Peek("id") != null)
            {
                int id = int.Parse((string)TempData.Peek("id"));
                DataSet ds = user.allproduct_status(id);
                ViewBag.Product = ds.Tables[0];
                List<String> imggUrls = new List<String>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    imggUrls.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
                }
                ViewBag.ProductImages = imggUrls;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        public ActionResult InitiatePayment(Home user, int id)
        {
            var key = "rzp_test_2TG19aA3c66eEv";
            var secret = "pUXmQ6Bmbfkjoz1yRmS607zC";
            DataSet ds = user.selectsubscription(id);
            ViewBag.suscription = ds.Tables[0];
            int amount = 0;
            foreach (System.Data.DataRow dr in ViewBag.suscription.Rows)
            {
                TempData["num_product"] = Convert.ToInt32(dr["num_product"]);
                TempData["num_bid"] = Convert.ToInt32(dr["num_bid"]);
                amount = Convert.ToInt32(dr["price"]);
            }
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToDecimal(amount) * 100);
            options.Add("currency", "USD");
            Order order = client.Order.Create(options);
            ViewBag.orderId = order["id"].ToString();
            return View("Payment");
        }
        public ActionResult InitiatePaymentProduct(Home user, int id)
        {
            var key = "rzp_test_2TG19aA3c66eEv";
            var secret = "pUXmQ6Bmbfkjoz1yRmS607zC";
            DataSet ds = user.select_product(id);
            ViewBag.suscription = ds.Tables[0];
            int amount = 0;
            foreach (System.Data.DataRow dr in ViewBag.suscription.Rows)
            {
                amount = Convert.ToInt32(dr["price"]);
            }
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToDecimal(amount) * 100);
            options.Add("currency", "USD");
            Order order = client.Order.Create(options);
            ViewBag.orderId = order["id"].ToString();
            return View("Payment");
        }
        public ActionResult InitiatePaymentProductCart(Home user, string payment)
        {
            var key = "rzp_test_2TG19aA3c66eEv";
            var secret = "pUXmQ6Bmbfkjoz1yRmS607zC";
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToDecimal(payment) * 100);
            options.Add("currency", "USD");
            Order order = client.Order.Create(options);
            ViewBag.orderId = order["id"].ToString();
            ViewBag.Price = Convert.ToDecimal(payment);
            return View("Payment");
        }
        public ActionResult Payment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature, Home add)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature); ;
            string paymentId = razorpay_payment_id;
            string orederId = razorpay_order_id;
            add.Payment(paymentId, orederId);
            try
            {
                int id = Convert.ToInt32(TempData["id"]);
                int product = Convert.ToInt32(TempData["num_product"]);
                int bid = Convert.ToInt32(TempData["num_bid"]);
                add.updateTotal(product,bid,id);
                Utils.verifyPaymentSignature(attributes);
                return View("PaymentSuccess");
            }
            catch (Exception ex)
            {
                return View("PaymentFailure");
            }
        }

        [HttpGet]
        public IActionResult ProductInsert()
        {
            if (TempData.Peek("id") != null)
            {

                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> ProductInsert(Home add, IFormFile formFile)
        {
            var image = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Products", formFile.FileName);
            using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
            string seriallizableString = image.ToString();
            TempData["p_image"] = seriallizableString;
            add.ProductImage = image.ToString();
            int id = int.Parse((string)TempData.Peek("id"));
            DataSet ds = add.SlectNumProduct(id);
            ViewBag.remainingProduct = ds.Tables[0];
            foreach (System.Data.DataRow row in ViewBag.remainingProduct.Rows)
            {
                if (Convert.ToInt32(row["num_product"]) < Convert.ToInt32(row["total_product"]))
                {
                    int product = Convert.ToInt32(row["num_product"]);
                    int number = add.productInsert(add.product_name, id, add.brand, add.color, add.condition, add.description, add.starting_bid, add.price, add.PType, add.ProductImage);
                    if (number != 0)
                    {
                        int remain = product + 1;
                        add.updateRemaining(remain, id);
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessageProduct");
                }
            }
            return RedirectToAction("Product");
        }
        [HttpGet]
        public IActionResult ReplyPage(Home user)
        {
            return View();
        }
        public IActionResult Delete_product(Home db, int id = 0)
        {
            int userId = int.Parse((string)TempData.Peek("id"));
            DataSet ds = db.SlectNumProduct(userId);
            ViewBag.remainingProduct = ds.Tables[0];
            foreach (System.Data.DataRow row in ViewBag.remainingProduct.Rows)
            {
                int product = Convert.ToInt32(row["num_product"]);
                int number = db.delete_product(id);
                if (number != 0)
                {
                    int remain = product - 1;
                    db.updateRemaining(remain, userId);
                }
            }
            return RedirectToAction("Product");
        }
        [HttpGet]
        public IActionResult Update_product(Home db, int id)
        {
            DataSet dataSet = db.select_product(id);
            ViewBag.updateData = dataSet.Tables[0];
            return View();
        }
        [HttpPost]
        public IActionResult Update_product(Home add, int id, int a = 0)
        {
            add.updateproduct(add.product_name, add.brand, add.color, add.condition, add.description, add.starting_bid, add.price,id);
            return RedirectToAction("Product");
        }
        [HttpGet]
        public IActionResult Bidder(Home user, int id, int a = 0)
        {
            int user_id = int.Parse((string)TempData.Peek("id"));
            DataSet db = user.selectBidder(id, user_id);
            ViewBag.bidder = db.Tables[0];
            DataSet dataSet = user.select_product(id);
            ViewBag.data = dataSet.Tables[0];
            DataSet ds = user.owner(user_id);
            ViewBag.owner = ds.Tables[0];
            return View();
        }


        [HttpGet]
        public IActionResult AddToCart(Home user)

        {
            if (TempData.Peek("id") != null)
            {
                int id = int.Parse((string)TempData.Peek("id"));
                DataSet ds = user.allproduct_cart(id);
                ViewBag.cartdata = ds.Tables[0];
                List<String> imggUrls = new List<String>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    imggUrls.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
                }
                ViewBag.ProductImages = imggUrls;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        [HttpGet]
        public IActionResult AddToWishList(Home user)

        {
            if (TempData.Peek("id") != null)
            {
                int id = int.Parse((string)TempData.Peek("id"));
                DataSet ds = user.allproduct_WishList(id);
                ViewBag.WishlistData = ds.Tables[0];
                List<String> imggUrls = new List<String>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    imggUrls.Add(Url.Content("~/Products/") + dr["p_image"].ToString());
                }
                ViewBag.ProductImages = imggUrls;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        [HttpGet]
        public IActionResult RemoveToWishList(Home user, int id)

        {
            int user_id = Convert.ToInt32(TempData.Peek("id"));
            user.removeWishlist(user_id, id);
            return RedirectToAction("AddToWishList");

        }
        [HttpGet]
        public IActionResult RemoveToCart(Home user, int id)

        {
            int user_id = Convert.ToInt32(TempData.Peek("id"));
            user.removeCart(user_id, id);
            return RedirectToAction("AddToCart");

        }
        [HttpGet]
        public IActionResult Addtocart_Product(Home ad, int id)
        {
            if (TempData.Peek("id") != null)
            {
                DataSet ds = ad.select_product(id);
                ViewBag.addtocart = ds.Tables[0];
                foreach (System.Data.DataRow row in ViewBag.addtocart.Rows)
                {
                    int user_id = Convert.ToInt32(TempData.Peek("id"));
                    ad.addtocart(user_id, id);
                }
                return RedirectToAction("AddToCart");
            }
            return RedirectToAction("Login");

        }
        public IActionResult RemoveQty(Home ad, int id)
        {
            int user_id = Convert.ToInt32(TempData.Peek("id"));
            if (TempData.Peek("id") != null)
            {
                DataSet ds = ad.selectCartedProduct(user_id, id);
                ViewBag.addtocart = ds.Tables[0];
                foreach (System.Data.DataRow row in ViewBag.addtocart.Rows)
                {
                    int qty = Convert.ToInt32(row["qty"]);
                    int final = qty - 1;
                    if (final == 0)
                    {
                        ad.removeCart(user_id, id);
                    }
                    else
                    {
                        ad.updateQty(user_id, id, final);
                    }
                }
                return RedirectToAction("AddToCart");
            }
            return RedirectToAction("Login");

        }
        public IActionResult AddQty(Home ad, int id)
        {
            int user_id = Convert.ToInt32(TempData.Peek("id"));
            if (TempData.Peek("id") != null)
            {
                DataSet ds = ad.selectCartedProduct(user_id, id);
                ViewBag.addtocart = ds.Tables[0];
                foreach (System.Data.DataRow row in ViewBag.addtocart.Rows)
                {
                    int qty = Convert.ToInt32(row["qty"]);
                    int final = qty + 1;
                    ad.updateQty(user_id, id, final);
                }
                return RedirectToAction("AddToCart");
            }
            return RedirectToAction("Login");

        }
        [HttpGet]
        public IActionResult WishList(Home ad, int id)
        {
            if (TempData.Peek("id") != null)
            {
                DataSet ds = ad.select_product(id);
                ViewBag.addtocart = ds.Tables[0];
                foreach (System.Data.DataRow row in ViewBag.addtocart.Rows)
                {
                    int user_id = Convert.ToInt32(TempData.Peek("id"));
                    ad.addtoWishList(user_id, id);
                }
                return RedirectToAction("AddToWishList");
            }
            return RedirectToAction("Login");

        }
        [HttpGet]
        public IActionResult SubPlan(Home user)

        {
            DataSet ds = user.allplan();
            ViewBag.subdata = ds.Tables[0];
            return View();

        }
        [HttpPost]
        public IActionResult SubPlan(Home user, int id)

        {
            DataSet ds = user.all_plan(id);
            ViewBag.subdata = ds.Tables[0];
            return RedirectToAction("Payment");

        }
        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}