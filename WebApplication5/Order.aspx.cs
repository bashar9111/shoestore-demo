using System;
using System.Linq;
namespace WebApplication5 { public partial class Order : DemoPage {
 protected void Page_Load(object sender,EventArgs e) { RequireLogin(); if(!IsPostBack) ViewState["OrderToken"]=Guid.NewGuid().ToString("N"); }
 protected override void OnPreRender(EventArgs e) {
  base.OnPreRender(e); lblSummary.Text=Cart.Count==0?"Your cart is empty. Browse shoes to add a product.":Server.HtmlEncode(string.Join(", ",Cart.Select(c=>DemoStore.Catalog.Single(p=>p.Id==c.Key).Name+" × "+c.Value)));
  lblTotal.Text="OMR "+DemoStore.Total(Cart).ToString("0.000",System.Globalization.CultureInfo.InvariantCulture);
  btnPlaceOrder.Enabled=Cart.Count>0; btnClear.Enabled=Cart.Count>0;
 }
 protected void ClearCart(object sender,EventArgs e) {Cart.Clear();ViewState["OrderToken"]=Guid.NewGuid().ToString("N");}
 protected void btnPlaceOrder_Click(object sender,EventArgs e) {
  if(!Request.IsAuthenticated) return;
  if(string.IsNullOrWhiteSpace(txtName.Text)||txtName.Text.Trim().Length>80) {lblOrder.Text="Enter a name of 1–80 characters.";return;}
  try { string id=DemoStore.Place(DemoStore.StoragePath,User.Identity.Name,txtName.Text,Cart,(string)ViewState["OrderToken"]); Cart.Clear(); lblOrder.Text="Demo order recorded: "+id+". No payment or delivery will take place."; ViewState["OrderToken"]=Guid.NewGuid().ToString("N"); }
  catch(ArgumentException ex) {lblOrder.Text=Server.HtmlEncode(ex.Message);}
 }
} }
