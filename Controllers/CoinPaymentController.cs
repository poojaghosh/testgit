using BitPayLight.Models.Invoice;
using BlockChainPayment.Common;
using BlockChainPayment.Models;
using BlockChainPayment.Models.AdvantageBO;
using KCPAdvantageCart.Models.AdvantageBL;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static BlockChainPayment.Models.Helper;



namespace BlockChainPayment.Controllers
{
    public class CoinPaymentController : Controller
    {
        UserRepository userRepository = new UserRepository();
        int flag = 0;
        
        // GET: CoinPayment
        public ActionResult Index()
        {
                     return View();
        }
        public ActionResult SuccessHandler(string orderNumber)
        {
            
        var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
         
            CommonFunction.SetCookie("CartOrderId", orderNumber);
            CommonFunction.SetCookie("CoinPaymentStatus", "True");
            //Success/PaymentSuccessPage 
            //var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var paidstatus = userRepository.UpdateCutomerOrderPaymentStatus(orderNumber, "CoinPayment");
            var RedirectUrl = baseUrl + "/Success/PaymentSuccessPage";
            flag = 1;
            userRepository.UpdatePaymentStatus(flag);
            
           
            return Redirect(RedirectUrl);
           
        }

        public ActionResult CancelOrder()
        {
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var RedirectUrl = baseUrl + "/Home/CheckOutPage";
            return Redirect(RedirectUrl);
        }

        [HttpPost]
        public ActionResult IPNHandler()
        {
            byte[] parameters;
            using (var stream = new MemoryStream())
            {
                //Request.Body.Clone(stream);
                parameters = stream.ToArray();
            }
            var strRequest = Encoding.ASCII.GetString(parameters);
            var ipnSecret = ConfigurationManager.AppSettings["IpnSecret"]; 

            if (Helper.VerifyIpnResponse(strRequest, Request.Headers["hmac"], ipnSecret,
                out Dictionary<string, string> values))
            {
                values.TryGetValue("first_name", out string firstName);
                values.TryGetValue("last_name", out string lastName);
                values.TryGetValue("email", out string email);
                values.TryGetValue("amount1", out string amount1);
                values.TryGetValue("subtotal", out string subtotal);
                values.TryGetValue("status", out string status);
                values.TryGetValue("status_text", out string statusText);

                var newPaymentStatus = Helper.GetPaymentStatus(status, statusText);

                switch (newPaymentStatus)
                {
                    case PaymentStatus.Pending:
                        {
                            //TODO: update order status and use logging mechanism
                            //order is pending                      
                        }
                        break;
                    case PaymentStatus.Authorized:
                        {
                            //order is authorized                      
                        }
                        break;
                    case PaymentStatus.Paid:
                        {
                            //order is paid                      
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //Issue Occurred with CoinPayments IPN  
            }

            //nothing should be rendered to visitor  
            return Content("");
        }
        private IDictionary<string, string> CreateQueryParameters(OrderEntity orderModel)
        {
            //get store location  
            
            var storeLocation = Request.Url.Scheme + "://" + Request.Url.Authority;
            var queryParameters = new Dictionary<string, string>()
            {
                //IPN settings  
                ["ipn_version"] = "1.0",
                ["cmd"] = "_pay_auto",
                ["ipn_type"] = "simple",
                ["ipn_mode"] = "hmac",
                ["merchant"] = ConfigurationManager.AppSettings["MerchantId"],
                ["allow_extra"] = "0",
                ["currency"] = "USD",
                ["amountf"] = orderModel.TotalCost?.ToString("N2"),
                ["item_name"] = "",

                //IPN, success and cancel URL  
                ["success_url"] = $"{storeLocation}/CoinPayments/SuccessHandler?orderNumber={orderModel.OrderId}",
                ["ipn_url"] = $"{storeLocation}/CoinPayments/IPNHandler",
                ["cancel_url"] = $"{storeLocation}/CoinPayments/CancelOrder",

                //order identifier                  
                ["custom"] = orderModel.OrderId,

                //billing info  
                ["first_name"] = "",
                ["last_name"] = "",
                ["email"] = orderModel.UserEmail,

            };
            return queryParameters;
        }


        public async Task<ActionResult> ProcessCheckout(string orderId, string userEmail)
        {
            OrderEntity _orderinfo = new OrderEntity();
            flag = 1;
            userRepository.UpdatePaymentStatus(flag);
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var redirectUrl="";
            var successurl = $"{baseUrl}/CoinPayment/SuccessHandler";
            var cancelurl = $"{baseUrl}/CoinPayment/CancelOrder";
            successurl = successurl + "?orderNumber=" + orderId;
            try
            {
                //get order information
                _orderinfo = userRepository.GetCustomerOrderInfo(userEmail, orderId);
                var queryParameters = CreateQueryParameters(_orderinfo);

                userEmail = "testKCSpartansData@gmail.com";
                redirectUrl = "https://www.coinpayments.net/index.php?ipn_version=1.0&cmd=_pay_auto&ipn_type=simple&ipn_mode=hmac&merchant=" + ConfigurationManager.AppSettings["MerchantId"].ToString() +
                    //"&allow_extra=0&currency=USD&amountf="+ _orderinfo.TotalCost +"&item_name=Order Id:" + _orderinfo.OrderId.ToString() + "&success_url=" + successurl + "&ipn_url=https%3A%2F%2Flocalhost%3A44397%2FCoinPayment%2FIPNHandler&cancel_url="+ cancelurl +"& custom=a8ae1cde-3b83-4ae9-b5e7-6efdd446b382&first_name=Kedar&last_name=Veerkar&email=" + userEmail + "&email=" + userEmail + "";
                    "&allow_extra=0&currency=USD&amountf=" + _orderinfo.TotalCost + "&item_name=Order Id:" + _orderinfo.OrderId.ToString() + "&success_url=" + successurl + "&ipn_url=https%3A%2F%2Flocalhost%3A44397%2FCoinPayment%2FIPNHandler&cancel_url=" + cancelurl + "& custom=a8ae1cde-3b83-4ae9-b5e7-6efdd446b382&first_name=Test&last_name=User&email=" + userEmail + "&address1=" + "Road no. 8" + "&address2=" + "3rd cross street" + "&city=" + "Green Valley" + "&country=" + "USA" + "&state=" + "AL" + "&zip=" + "35006" + "&phone=" + "1234567890" +"";


                Redirect(redirectUrl);
            }
            catch(Exception ex)
            {

            }
            

            
            return Json(new { Status = true, ReturnUrl = redirectUrl, InvoiceId = "123" }, JsonRequestBehavior.AllowGet);
        }

       
    }
}