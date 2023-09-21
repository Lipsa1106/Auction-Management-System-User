﻿
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace demo.Models
{
    public class Home
    {
        SqlConnection con = new SqlConnection("Data Source=.\\SQLExpress;Database=hi-tech;User Id=sa;pwd=palak@123");
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string product_name { get; set; }
        public string brand { get; set; }
        public string color { get; set; }
        public string price { get; set; }
        public string condition { get; set; }
        public string description { get; set; }
        public string starting_bid { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string f_name { get; set; }
        public string l_name { get; set; }
        public string mobile { get; set; }
        public string message { get; set; }
        public string bid_value { get; set; }
        public string bid_time { get; set; }
        public string bidder_name { get; set; }
        public string PType { get; set; }
        public int register(string name, string email, string password)
        {
            SqlCommand cmd = new SqlCommand("insert into [dbo].[Register] (name,email,password,status,report)values('" + name + "','" + email + "','" + password + "','" + true + "','" + false + "')", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public int upload_product(int user_id, int total_product, int num_product, int num_bid, int total_bid)
        {
            con.Close();
            SqlCommand cmd = new SqlCommand("insert into [dbo].[UploadProduct] (user_id,total_product,num_product,num_bid,total_bid)values('" + user_id + "','" + total_product + "','" + num_product + "','" + num_bid + "','" + total_bid + "')", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public DataSet login(string email, string password, string status)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Register] where email='" + email + "' and password='" + password + "' and status='" + status + "'", con);
            con.Open();
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet allproduct()
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Table_1] where pType='" + "auction" + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet NormalProduct()
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Table_1] where pType='" + "normal" + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int productInsert(string name, int id, string brand, string color, string conditin, string des, string bid, string price, string pType)
        {
            SqlCommand cmd = new SqlCommand("insert into [dbo].[Table_1] (product_name,user_id,brand,color,condition,description,starting_bid,price,status,report,cart,wishlist,auctionLive,pType)values('" + name + "','" + id + "','" + brand + "','" + color + "','" + conditin + "','" + des + "','" + bid + "','" + price + "','" + false + "','" + "False" + "','" + "false" + "','" + "false" + "','" +
                "true" + "','" + pType + "')", con);
            con.Open();
            return cmd.ExecuteNonQuery();

        }
        public DataSet select_product(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Table_1] where id='" + id + "'", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int contactUs(int id, string f_name, string l_name, string email, string mobile, string message)
        {
            SqlCommand cmd = new SqlCommand("insert into [dbo].[ContactUs] (user_id,f_name,l_name,email,mobile,message)values('" + id + "','" + f_name + "','" + l_name + "','" + email + "','" + mobile + "','" + message + "')", con);
            con.Open();
            return cmd.ExecuteNonQuery();

        }
        public DataSet allTeam()
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[AboutUs]", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet allblog()
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Blog]", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet allproduct_status(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Table_1] where  user_id = '" + id + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int SubmitBid(int id, int product_id, string bid_value, string bid_time, string bidder_name)
        {
            con.Close();
            SqlCommand cmd = new SqlCommand("IF EXISTS " +
                "(SELECT * from [dbo].[SubmitBid1] where user_id='" + id + "' and product_id='" + product_id + "')" +
                " BEGIN " +
                "SELECT '" + 0 + "' " +
                "END " +
                "ELSE " +
                "BEGIN " +
                "insert into [dbo].[SubmitBid1](user_id,product_id,bid_value,bid_time,bidder_name,winner)values('" + id + "','" + product_id + "','" + bid_value + "','" + bid_time + "','" + bidder_name + "','" + "false" + "') " +
                "END", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public int delete_product(int id)
        {
            SqlCommand cmd = new SqlCommand("delete from [dbo].[Table_1] where id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public int updateproduct(string name, int id, string brand, string color, string conditin, string des, string bid, string price, string start, string end, int pid)
        {
            SqlCommand cmd = new SqlCommand("update [dbo].[Table_1] set product_name='" + name + "',brand='" + brand + "',color='" + color + "',condition='" + conditin + "',description='" + des + "',starting_bid='" + bid + "',price='" + price + "',start_time='" + start + "',end_time='" + end + "' where id='" + pid + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public DataSet owner(int user_id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Register] where id='" + user_id + "'", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet selectBidder(int id, int user_id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[SubmitBid1] where  product_id = '" + id + "' and user_id='" + user_id + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet winner(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[SubmitBid1] where product_id='" + id + "' ORDER BY bid_value DESC OFFSET 0 ROWS FETCH FIRST 1 ROWS ONLY   ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet winnerBid(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[SubmitBid1] where product_id='" + id + "' ORDER BY bid_value ASC", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int updateWinner(int id)
        {
            SqlCommand cmd = new SqlCommand("update [dbo].[SubmitBid1] set winner='" + "True" + "' where bid_id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public int updateAuction(int id)
        {
            con.Close();
            SqlCommand cmd = new SqlCommand("update [dbo].[Table_1] set auctionLive='" + "false" + "' where id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public DataSet SelectLastRecord()
        {
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM [dbo].[Register]  ORDER BY id DESC OFFSET 0 ROWS FETCH FIRST 1 ROWS ONLY ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int addtocart(string status, int id)
        {
            SqlCommand cmd = new SqlCommand("update [dbo].[Table_1] set cart='" + status + "' where id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public int addtoWishList(string status, int id)
        {
            SqlCommand cmd = new SqlCommand("update [dbo].[Table_1] set wishList ='" + status + "' where id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public DataSet allproduct_cart(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Table_1] where cart = '" + "True" + "' and user_id = '" + id + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet allproduct_WishList(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Table_1] where wishlist = '" + "True" + "' and user_id = '" + id + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet SlectNumProduct(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[UploadProduct] where user_id = '" + id + "' ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int updateRemaining(int prodcut, int id)
        {
            con.Close();
            SqlCommand cmd = new SqlCommand("update [dbo].[UploadProduct] set num_product ='" + prodcut + "' where user_id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
        public DataSet allplan()
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[PackageMaster] ", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public DataSet all_plan(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[PackageMaster]  where id = '" + id + "'", con);
            SqlDataAdapter dn = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            dn.Fill(ds);
            return ds;
        }
        public int updateRemainingBid(int bid, int id)
        {
            con.Close();
            SqlCommand cmd = new SqlCommand("update [dbo].[UploadProduct] set num_bid ='" + bid + "' where user_id='" + id + "'", con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
