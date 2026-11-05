using System;
using System.Collections.Generic;
using System.IO;
using WebApplication5;
class Tests {
 static int count;
 static void Check(bool pass,string label){if(!pass)throw new Exception(label);Console.WriteLine("PASS: "+label);count++;}
 static void Main(string[] args){
 string path=args[0];
 Check(!File.Exists(path),"isolated test store");
 Check(DemoStore.Register(path,"a","bad","short")!=null,"reject invalid registration");
 Check(DemoStore.Register(path,"demo_one","one@example.test","Testing-Only-1234")==null,"create account");
 Check(DemoStore.Register(path,"DEMO_ONE","one@example.test","Testing-Only-1234")!=null,"reject case-insensitive duplicate");
 Check(DemoStore.Authenticate(path,"demo_one","wrong")==null,"reject wrong password");
 Check(DemoStore.Authenticate(path,"unknown","wrong")==null,"reject unknown user");
 Check(DemoStore.Authenticate(path,"DEMO_ONE","Testing-Only-1234")=="demo_one","authenticate persisted account");
 Check(!File.ReadAllText(path).Contains("Testing-Only-1234"),"password not stored in plaintext");
 Check(DemoStore.Register(path,"demo_two","two@example.test","Testing-Only-5678")==null,"create second account");
 var cart=new Dictionary<int,int>{{1,2},{2,1}};
 Check(DemoStore.Total(cart)==125m,"calculate prices on server");
 bool rejected=false;try{DemoStore.Total(new Dictionary<int,int>{{999,1}});}catch(ArgumentException){rejected=true;}Check(rejected,"reject unknown product");
 rejected=false;try{DemoStore.Total(new Dictionary<int,int>{{1,11}});}catch(ArgumentException){rejected=true;}Check(rejected,"reject excessive quantity");
 string token=Guid.NewGuid().ToString("N");
 DemoStore.Place(path,"demo_one","Fictional Customer",cart,token);
 DemoStore.Place(path,"demo_one","Fictional Customer",cart,token);
 Check(DemoStore.Orders(path,"demo_one").Count==1,"prevent duplicate order token");
 Check(DemoStore.Orders(path,"demo_one")[0].Amount==125m,"persist order amount");
 Check(DemoStore.Orders(path,"demo_two").Count==0,"isolate order history by user");
 rejected=false;try{DemoStore.Place(path,"demo_two","Other",cart,token);}catch(ArgumentException){rejected=true;}Check(rejected,"reject another user's order token");
 rejected=false;try{DemoStore.Place(path,"missing","Other",cart,Guid.NewGuid().ToString("N"));}catch(ArgumentException){rejected=true;}Check(rejected,"reject unauthenticated account");
 Console.WriteLine("TOTAL: "+count+" passed");
 }
}
