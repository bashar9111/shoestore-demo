using System;
namespace WebApplication5 { public partial class OrderManagement : DemoPage {
 protected void Page_Load(object sender,EventArgs e) {RequireLogin();if(Request.IsAuthenticated){ GridView1.DataSource=DemoStore.Orders(DemoStore.StoragePath,User.Identity.Name).SelectForGrid(); GridView1.DataBind(); }}
} public static class OrderGrid {public static object SelectForGrid(this System.Collections.Generic.List<DemoOrder> orders) {return System.Linq.Enumerable.Select(orders,o=>new {o.OrderID,o.CustomerName,o.OrderDate,o.Amount,o.Status,o.Items});}} }
