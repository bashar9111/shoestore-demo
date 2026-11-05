using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
[Serializable] public class Result { public string Html;public int Status=200;public List<string> Cookies=new List<string>(); }
public class Worker:SimpleWorkerRequest {
 string body,cookie;Result result;
 public Worker(string page,string body,string cookie,StringWriter output,Result result):base(page,"",output){this.body=body;this.cookie=cookie;this.result=result;}
 public override string GetHttpVerbName(){return body==null?"GET":"POST";}
 public override string GetKnownRequestHeader(int index){if(index==HeaderCookie)return cookie;if(index==HeaderContentType&&body!=null)return "application/x-www-form-urlencoded";if(index==HeaderContentLength&&body!=null)return Encoding.UTF8.GetByteCount(body).ToString();return base.GetKnownRequestHeader(index);}
 public override byte[] GetPreloadedEntityBody(){return body==null?null:Encoding.UTF8.GetBytes(body);}
 public override bool IsEntireEntityBodyIsPreloaded(){return true;}
 public override int GetTotalEntityBodyLength(){return body==null?0:Encoding.UTF8.GetByteCount(body);}
 public override void SendStatus(int code,string description){result.Status=code;base.SendStatus(code,description);}
 public override void SendKnownResponseHeader(int index,string value){if(index==HeaderSetCookie)result.Cookies.Add(value);base.SendKnownResponseHeader(index,value);}
}
public class FlowHost:MarshalByRefObject {
 public Result Run(string page,string body,string cookie){var r=new Result();var sw=new StringWriter();HttpRuntime.ProcessRequest(new Worker(page,body,cookie,sw,r));r.Html=sw.ToString();return r;}
}
class FlowTests {
 static Dictionary<string,string> cookies=new Dictionary<string,string>();static FlowHost host;static int count;
 static Result Run(string page,string body){var cookie=new StringBuilder();foreach(var c in cookies)cookie.Append(c.Key+"="+c.Value+"; ");var r=host.Run(page,body,cookie.ToString());foreach(var c in r.Cookies){var pair=c.Split(';')[0];int i=pair.IndexOf('=');cookies[pair.Substring(0,i)]=pair.Substring(i+1);}if(r.Status>=500)throw new Exception("Server error "+page+": "+r.Html);return r;}
 static string Post(string html,params string[] values){var parts=new List<string>();foreach(Match m in Regex.Matches(html,"<input[^>]*type=\"hidden\"[^>]*>")){string tag=m.Value;var n=Regex.Match(tag,"name=\"([^\"]+)\"");var v=Regex.Match(tag,"value=\"([^\"]*)\"");if(n.Success)parts.Add(HttpUtility.UrlEncode(HttpUtility.HtmlDecode(n.Groups[1].Value))+"="+HttpUtility.UrlEncode(HttpUtility.HtmlDecode(v.Groups[1].Value)));}for(int i=0;i<values.Length;i+=2)parts.Add(HttpUtility.UrlEncode(values[i])+"="+HttpUtility.UrlEncode(values[i+1]));return string.Join("&",parts);}
 static void Check(bool pass,string label){if(!pass)throw new Exception(label);Console.WriteLine("PASS: "+label);count++;}
 static void Main(string[] args){host=(FlowHost)ApplicationHost.CreateApplicationHost(typeof(FlowHost),"/",args[0]);
 Check(Run("OrderManagement.aspx",null).Status==302,"anonymous history redirects to login");
 var r=Run("Register.aspx",null);r=Run("Register.aspx",Post(r.Html,"ctl00$MainContent$txtUsername","flow_user","ctl00$MainContent$txtEmail","flow@example.test","ctl00$MainContent$txtPassword","Fictional-Password-123","ctl00$MainContent$txtConfirm","Fictional-Password-123","ctl00$MainContent$btnRegister","Create Account"));Check(r.Html.Contains("Account created."),"registration postback saves account");
 r=Run("Products.aspx",null);r=Run("Products.aspx",Post(r.Html,"ctl00$MainContent$btnSport","Add to cart"));Check(r.Status==302,"add product postback redirects");
 r=Run("Login.aspx",null);r=Run("Login.aspx",Post(r.Html,"ctl00$MainContent$txtUsername","flow_user","ctl00$MainContent$txtPassword","Fictional-Password-123","ctl00$MainContent$btnLogin","Login"));Check(r.Status==302&&cookies.ContainsKey(".ASPXAUTH"),"login issues auth cookie");
 r=Run("Order.aspx",null);Check(r.Status==200&&r.Html.Contains("OMR 45.000"),"cart survives login with correct total");
 r=Run("Order.aspx",Post(r.Html,"ctl00$MainContent$txtName","Fictional Buyer","ctl00$MainContent$btnPlaceOrder","Place demo order"));Check(r.Html.Contains("Demo order recorded:"),"checkout postback records order");Check(r.Html.Contains("Your cart is empty"),"checkout clears cart");
 r=Run("OrderManagement.aspx",null);Check(r.Html.Contains("Fictional Buyer")&&r.Html.Contains("45.000"),"authenticated history renders saved order");
 r=Run("Home.aspx",null);r=Run("Home.aspx",Post(r.Html,"__EVENTTARGET","ctl00$LogoutButton","__EVENTARGUMENT",""));Check(r.Status==302,"sign out postback redirects");Check(Run("OrderManagement.aspx",null).Status==302,"signed out account cannot view orders");
 Console.WriteLine("TOTAL: "+count+" passed");}
}

