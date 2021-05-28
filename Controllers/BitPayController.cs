using BitPayLight;
using BitPayLight.Models.Invoice;
using BlockChainPayment.Common;
using BlockChainPayment.Models.AdvantageBO;
using KCPAdvantageCart.Models.AdvantageBL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BlockChainPayment.Controllers
{
    public class BitPayController : Controller
    {
        UserRepository userRepository = new UserRepository();
        BitPay bitPay = new BitPay(token: ConfigurationManager.AppSettings["BitPay_Token"], environment: Env.Test);
       
        // GET: BitPay
        public async Task<ActionResult> BitCoinPay(string orderId, string userEmail)
        {
            OrderEntity _orderinfo = new OrderEntity();

            //get order information
            _orderinfo = userRepository.GetCustomerOrderInfo(userEmail, orderId);
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            var invoice = await bitPay.CreateInvoice(new Invoice
            {
                Price = Convert.ToDouble(_orderinfo.TotalCost),
                Currency = "USD",
                PosData = Convert.ToString(_orderinfo.UserId),
                OrderId = _orderinfo.OrderId,
                RedirectUrl= $"{baseUrl}/Success/PaymentSuccessPage"
                //RedirectUrl = $"{baseUrl}/Home/CheckOutPage" //return back url
            });

            CommonFunction.SetCookie("BitPayInvoiceId", invoice.Id);

            return Json(new { Status = true, ReturnUrl = invoice.Url, InvoiceId=invoice.Id }, JsonRequestBehavior.AllowGet);
        }

        //"Voaz9PUuVa2KKdascB3iGq"
        public async Task<ActionResult> BitPayInvoiceStatus(string invoiceId, string orderId)
        {
            bool paidstatus = false;
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            if (!string.IsNullOrWhiteSpace(invoiceId) && !string.IsNullOrWhiteSpace(orderId))
            {
                //get invoice status
                Invoice invoice = await bitPay.GetInvoice(invoiceId);
                string url = string.Empty;

                // payment success
                if (invoice.Status == "paid")
                {
                    paidstatus = userRepository.UpdateCutomerOrderPaymentStatus(orderId, "BitPay");
                    url = $"{baseUrl}/Home/PaymentSuccessPage?orderId={orderId}";
                }

                return Json(new { Status = paidstatus, PaymentStatus = invoice.Status, ReturnUrl = url }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = paidstatus, PaymentStatus = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}