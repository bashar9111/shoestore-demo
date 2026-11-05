using System;
namespace WebApplication5 { public partial class Products : DemoPage {
 protected void AddToCart(object sender,EventArgs e) {
  int id=int.Parse(((System.Web.UI.WebControls.Button)sender).CommandArgument);
  if(!Cart.ContainsKey(id)) Cart[id]=0;
  if(Cart[id]<10) Cart[id]++;
  Response.Redirect("Order.aspx");
 }
} }
