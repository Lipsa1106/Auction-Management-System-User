﻿using demo.Models;
using HiTech.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
        [HttpPost]
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
                return RedirectToAction("ErrorMessageProduct");
            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Bid(Home user, int id, int a = 0)
        {
            int user_id;
            if (TempData.Peek("id") != null)
            {
                DataSet dataSet = user.select_product(id);
                ViewBag.data = dataSet.Tables[0];
                foreach (System.Data.DataRow row in ViewBag.data.Rows)
                {
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
                    try
                    {
                        int number = user.SubmitBid(user_id, id, user.bid_value, user.bid_time, user.bidder_name);
                        if (number != 0)
                        {
                            int remain = RemainingBid + 1;
                            user.updateRemainingBid(remain, user_id);

                        }
                    }
                    catch
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
        public IActionResult AboutUs(Home user)
        {
            DataSet ds = user.allTeam();
            ViewBag.data = ds.Tables[0];
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
            return View();
        }
        public IActionResult Buy(Home user)
        {
            DataSet ds = user.allproduct();
            ViewBag.data = ds.Tables[0];
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
                return View();
            }
            else
            {
                return RedirectToAction("Login");
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
        public IActionResult ProductInsert(Home add)
        {
            //var image = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim();
            //var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","NewProduct",formFile.FileName);
            //using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
            //{
            //    await formFile.CopyToAsync(stream);
            //}
            //string seriallizableString = image.ToString();
            //TempData["p_image"]= seriallizableString;
            //add.product_image = image.ToString();
            int id = int.Parse((string)TempData.Peek("id"));
            DataSet ds = add.SlectNumProduct(id);
            ViewBag.remainingProduct = ds.Tables[0];
            foreach (System.Data.DataRow row in ViewBag.remainingProduct.Rows)
            {
                if (Convert.ToInt32(row["num_product"]) < Convert.ToInt32(row["total_product"]))
                {
                    int product = Convert.ToInt32(row["num_product"]);
                    int number = add.productInsert(add.product_name, id, add.brand, add.color, add.condition, add.description, add.starting_bid, add.price, add.start_time, add.end_time, add.report, add.cart);
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
            ViewBag.data = dataSet.Tables[0];
            return View();
        }
        [HttpPost]
        public IActionResult Update_product(Home add, int id, int a = 0)
        {

            int user_id = int.Parse((string)TempData.Peek("id"));
            add.updateproduct(add.product_name, user_id, add.brand, add.color, add.condition, add.description, add.starting_bid, add.price, add.start_time, add.end_time, id);
            return RedirectToAction("Product");
        }
        [HttpGet]
        public IActionResult Bidder(Home user, int id, int a = 0)
        {
            int user_id;
            DataSet dataSet = user.select_product(id);
            ViewBag.data = dataSet.Tables[0];
            foreach (System.Data.DataRow row in ViewBag.data.Rows)
            {
                user_id = Convert.ToInt32(row["user_id"]);
                DataSet ds = user.owner(user_id);
                ViewBag.owner = ds.Tables[0];
            }
            DataSet db = user.selectBidder(id);
            ViewBag.bidder = db.Tables[0];
            return View();
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
                    if (row["cart"].ToString() == "false")
                    {
                        ad.addtocart("True", id);
                    }
                    else
                    {
                        ad.addtocart("false", id);
                    }
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
                    if (row["wishlist"].ToString() == "false")
                    {
                        ad.addtoWishList("True", id);
                    }
                    else
                    {
                        ad.addtoWishList("false", id);
                    }
                }
                return RedirectToAction("AddToWishList");
            }
            return RedirectToAction("Login");

        }
        [HttpGet]
        public IActionResult AddToCart(Home user)

        {
            int id = int.Parse((string)TempData.Peek("id"));
            DataSet ds = user.allproduct_cart(id);
            ViewBag.cartdata = ds.Tables[0];
            return View();

        }
        [HttpGet]
        public IActionResult AddToWishList(Home user)

        {
            int id = int.Parse((string)TempData.Peek("id"));
            DataSet ds = user.allproduct_WishList(id);
            ViewBag.WishlistData = ds.Tables[0];
            return View();

        }
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}