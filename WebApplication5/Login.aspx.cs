using System;
using System.Web.Security;
namespace WebApplication5 { public partial class Login : DemoPage {
 protected void btnLogin_Click(object sender,EventArgs e) {
  DateTime? next=Session["NextLogin"] as DateTime?;
  if(next.HasValue&&next.Value>DateTime.UtcNow) {lblMessage.Text="Please wait a moment before trying again.";return;}
  Session["NextLogin"]=DateTime.UtcNow.AddSeconds(2);
  string user=DemoStore.Authenticate(DemoStore.StoragePath,txtUsername.Text,txtPassword.Text);
  if(user==null) {lblMessage.Text="Username or password is incorrect.";return;}
  Session.Remove("NextLogin"); FormsAuthentication.SetAuthCookie(user,chkRemember.Checked); Response.Redirect("Products.aspx");
 }
} }
