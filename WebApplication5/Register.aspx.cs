using System;
namespace WebApplication5 { public partial class Register : DemoPage {
 protected void btnRegister_Click(object sender,EventArgs e) {
  Page.Validate(); if(!Page.IsValid) return;
  if(txtPassword.Text!=txtConfirm.Text) {lblMessage.Text="Passwords do not match.";return;}
  string error=DemoStore.Register(DemoStore.StoragePath,txtUsername.Text,txtEmail.Text,txtPassword.Text);
  lblMessage.Text=error??"Account created. You can now sign in.";
 }
} }
