using BitPayLight;
using BitPayLight.Models.Invoice;
using BlockChainPayment.Common;
using BlockChainPayment.Models.AdvantageBO;
using KCPAdvantageCart.Models.AdvantageBL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BlockChainPayment.Controllers
{
    public class SuccessController : Controller
    {

        UserRepository userRepository = new UserRepository();
        BitPay bitPay = new BitPay(token: ConfigurationManager.AppSettings["BitPay_Token"], environment: Env.Test);
        // GET: Success

        public ActionResult PaymentSuccessPage()
        {
            OrderEntity _orderinfo = new OrderEntity();

            string invoiceId = string.Empty;
            string orderId = string.Empty;
            string coinPaymentStatus = string.Empty;

            invoiceId = CommonFunction.GetCookie("BitPayInvoiceId");

            orderId = CommonFunction.GetCookie("CartOrderId");
            coinPaymentStatus = CommonFunction.GetCookie("CoinPaymentStatus");
            bool coinPaymentStatusFlag = false;
            bool bitPayInvoiceStatus = false;

            if (coinPaymentStatus.ToString().ToLower() == "true")
                coinPaymentStatusFlag = true;

            if (!string.IsNullOrWhiteSpace(invoiceId))
            {
                bitPayInvoiceStatus = BitPayInvoiceStatus(invoiceId, orderId);
            }

            if (bitPayInvoiceStatus || coinPaymentStatusFlag)
            {
                string userEmail = "chandan.kumar@kcc.com";
                //get order information
                _orderinfo = userRepository.GetCustomerOrderInfo(userEmail, orderId);
                CommonFunction.DeleteCookie("BitPayInvoiceId");
                CommonFunction.DeleteCookie("CoinPaymentStatus");
            }
            string _transactionId = Convert.ToString(Guid.NewGuid().ToString("N").Substring(0, 12)).ToUpper();
            _orderinfo.PaidTransactionId = _transactionId;
            CommonFunction.DeleteCookie("CartOrderId");
            return View(_orderinfo);
        }
        public JsonResult GetOrderInformationforSuccessPages(string orderId)
        {
            string userEmail = "chandan.kumar@kcc.com";
            OrderEntity _orderinfo = new OrderEntity();

            //get order information
            _orderinfo = userRepository.GetCustomerOrderInfo(userEmail, orderId);

            return Json(JsonConvert.SerializeObject(_orderinfo));
        }
        private bool BitPayInvoiceStatus(string invoiceId, string orderId)
        {
            bool paidstatus = false;
            if (!string.IsNullOrWhiteSpace(invoiceId) && !string.IsNullOrWhiteSpace(orderId))
            {
                //get invoice status
                //Invoice invoice = await bitPay.GetInvoice(invoiceId).ConfigureAwait(false);

                string url = string.Empty;
                string status = "paid";

                // payment success
                if (status == "paid")
                {
                    if (userRepository.UpdateCutomerOrderPaymentStatus(orderId, "BitPay"))
                    {
                        paidstatus = true;
                    }
                }
            }
            return paidstatus;
        }
    }
}