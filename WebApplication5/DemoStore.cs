using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
namespace WebApplication5 {
 public class DemoUser { public string Name; public string Email; public string Salt; public string Hash; }
 public class DemoOrder { public string OrderID; public string Username; public string CustomerName; public string OrderDate; public decimal Amount; public string Status; public string Items; }
 public class StoreData { public List<DemoUser> Users = new List<DemoUser>(); public List<DemoOrder> Orders = new List<DemoOrder>(); }
 public class Product { public int Id; public string Name; public decimal Price; public string Color; }
 public static class DemoStore {
  private static readonly object Gate=new object();
  public static readonly Product[] Catalog={
   new Product{Id=1,Name="Sports Shoes",Price=45,Color="#ad493e"},new Product{Id=2,Name="Casual Shoes",Price=35,Color="#334c55"},
   new Product{Id=3,Name="Formal Shoes",Price=55,Color="#79573c"},new Product{Id=4,Name="Classic Shoes",Price=40,Color="#34363c"},new Product{Id=5,Name="Leather Shoes",Price=60,Color="#946b40"}};
  public static string StoragePath { get { return System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/demo-store.xml"); } }
  private static StoreData Read(string path) { if(!File.Exists(path)) return new StoreData(); using(var f=File.OpenRead(path)) return (StoreData)new XmlSerializer(typeof(StoreData)).Deserialize(f); }
  private static void Save(string path,StoreData data) { Directory.CreateDirectory(Path.GetDirectoryName(path)); string temp=path+".tmp"; using(var f=File.Create(temp)) new XmlSerializer(typeof(StoreData)).Serialize(f,data); if(File.Exists(path)) File.Replace(temp,path,null); else File.Move(temp,path); }
  private static byte[] Hash(string password,byte[] salt) { using(var k=new Rfc2898DeriveBytes(password,salt,600000,HashAlgorithmName.SHA256)) return k.GetBytes(32); }
  public static bool ValidEmail(string email) { return email!=null && email.Length<=120 && Regex.IsMatch(email,@"^[^\s@]+@[^\s@]+\.[^\s@]+$"); }
  public static string Register(string path,string name,string email,string password) {
   name=(name??"").Trim(); email=(email??"").Trim();
   if(!Regex.IsMatch(name,@"^[A-Za-z0-9_]{3,30}$")) return "Use 3–30 letters, numbers or underscores for your username.";
   if(!ValidEmail(email)) return "Enter a valid email address.";
   if(password==null || password.Length<12 || password.Length>128) return "Use a password of 12–128 characters.";
   byte[] salt=new byte[16]; using(var rng=RandomNumberGenerator.Create()) rng.GetBytes(salt);
   string hash=Convert.ToBase64String(Hash(password,salt));
   lock(Gate) { var data=Read(path); if(data.Users.Any(u=>u.Name.Equals(name,StringComparison.OrdinalIgnoreCase))) return "That username is already in use.";
    data.Users.Add(new DemoUser{Name=name,Email=email,Salt=Convert.ToBase64String(salt),Hash=hash}); Save(path,data); } return null;
  }
  public static string Authenticate(string path,string name,string password) {
   if(string.IsNullOrWhiteSpace(name)||password==null||password.Length>128) return null;
   DemoUser user; lock(Gate) user=Read(path).Users.FirstOrDefault(u=>u.Name.Equals(name.Trim(),StringComparison.OrdinalIgnoreCase));
   byte[] salt=user==null?new byte[16]:Convert.FromBase64String(user.Salt); byte[] actual=Hash(password,salt); byte[] expected=user==null?new byte[32]:Convert.FromBase64String(user.Hash);
   int diff=0; for(int i=0;i<32;i++) diff|=actual[i]^expected[i]; return user!=null&&diff==0?user.Name:null;
  }
  public static decimal Total(Dictionary<int,int> cart) { decimal total=0; foreach(var item in cart) { var product=Catalog.SingleOrDefault(x=>x.Id==item.Key); if(product==null||item.Value<1||item.Value>10) throw new ArgumentException("Invalid cart."); total+=product.Price*item.Value; } return total; }
  public static string Place(string path,string user,string name,Dictionary<int,int> cart,string token) {
   if(string.IsNullOrWhiteSpace(user)||string.IsNullOrWhiteSpace(name)||name.Length>80||cart.Count==0||!Regex.IsMatch(token??"",@"^[a-f0-9]{32}$")) throw new ArgumentException("Invalid order.");
   decimal total=Total(cart); lock(Gate) { var data=Read(path); if(!data.Users.Any(u=>u.Name==user)) throw new ArgumentException("Please sign in again.");
    var old=data.Orders.FirstOrDefault(o=>o.OrderID==token); if(old!=null) {if(old.Username!=user) throw new ArgumentException("Invalid order.");return old.OrderID;}
    data.Orders.Add(new DemoOrder{OrderID=token,Username=user,CustomerName=name.Trim(),OrderDate=DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'"),Amount=total,Status="Demo — received",Items=string.Join(", ",cart.Select(c=>Catalog.Single(x=>x.Id==c.Key).Name+" × "+c.Value))}); Save(path,data); return token; }
  }
  public static List<DemoOrder> Orders(string path,string user) { lock(Gate) return Read(path).Orders.Where(o=>o.Username==user).Reverse().ToList(); }
 }
 public class DemoPage : System.Web.UI.Page {
  protected Dictionary<int,int> Cart { get { var c=Session["Cart"] as Dictionary<int,int>; if(c==null) Session["Cart"]=c=new Dictionary<int,int>(); return c; } }
  protected void RequireLogin() { if(!Request.IsAuthenticated) { Response.Redirect("Login.aspx",false); Context.ApplicationInstance.CompleteRequest(); } }
 }
}
