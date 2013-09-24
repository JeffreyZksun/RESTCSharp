using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace restcsharp
{
    class Sample
    {
        public static int  Main(){

            RestAPI restAPI = new RestAPI() { APIEndPoint = "http://127.0.0.1:3000" };
            var res_id = "1227f8e0-2367-11e3-95a6-d5147a878154";
            JObject order = null;

            // Create restaurant
            Console.WriteLine(">> Create restaurant");
            {
                // {"name":"湘菜馆","address":" No. 34 Tianlin Rd.","description":"Cool","telphones":["15933822749","021-66251973"],"logo":""}
                JObject requestData = new JObject();
                requestData.Add("name", "湘菜馆");
                requestData.Add("address", "No. 34 Tianlin Rd.");
                requestData.Add("description", "正宗湘菜");
                var telphones = new JArray();
                telphones.Add("15933822749");
                telphones.Add("021-66251973");
                requestData.Add("telphones", telphones);
                requestData.Add("logo", "");

                var uri = "restaurants";
                var method = "post";

                JObject response = null;
                bool ret = restAPI.SendHttpRequest(uri, method, requestData, out response);
                if (!ret)
                {
                    Console.Write("fail");
                    return 1;
                }
                else{
                    var strBody = JsonConvert.SerializeObject(response);
                    Console.WriteLine(strBody);
                    //res_id = response["id"].ToString();
                }
            }
            // Get order
            Console.WriteLine(">> Get order");
            {
                
                var uri = "restaurants/" + res_id + "/orders";
                var method = "GET";

                JArray response = null;
                bool ret = restAPI.SendHttpRequest(uri, method, null, out response);
                if (!ret)
                {
                    Console.Write("fail");
                    return 1;
                }
                else
                {
                    var strBody = JsonConvert.SerializeObject(response);
                    Console.WriteLine(strBody);

                    order = response[0] as JObject; 
                }
            }

            // Update order
            Console.WriteLine(">> Update order");
            {
                // {"items":[{"item":{"type":"course","id":"68ca6820-24db-11e3-87c1-f12296dee4f7"},"quantity":2}]}

                // Get int
                // http://stackoverflow.com/questions/9589218/get-value-from-jtoken-that-may-not-exist-best-practices                
                var status = order["status"].ToString();

                // The link to modify the jobject
                // http://stackoverflow.com/questions/3206345/how-can-i-best-utilize-json-net-to-modify-parts-of-an-existing-json-object
                JValue newStatus = new JValue("printed");
                order["status"].Replace(newStatus);

                var uri = "orders/" + order["id"];
                var method = "PUT";

                JObject response = null;
                bool ret = restAPI.SendHttpRequest(uri, method, order, out response);
                if (!ret)
                {
                    Console.Write("fail");
                    return 1;
                }
                else
                {
                    var strBody = JsonConvert.SerializeObject(response);
                    Console.WriteLine(strBody);
                }
            } 

            return 0;
        }
    }
}
