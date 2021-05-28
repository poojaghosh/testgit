using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCPAdvantageCart.Models.AdvantageBO;
using KCPAdvantageCart.Models.AdvantageBL;
using System.Runtime.Remoting.Messaging;
using BlockChainPayment.Models.AdvantageBO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BitPayLight.Models.Invoice;
using BlockChainPayment.Common;

namespace KCPAdvantageCart.Controllers
{
    public class HomeController : Controller
    {
        UserRepository userRepository = new UserRepository();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {

            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetCartDetails(string id)
        {
            string email = "chandan.kumar@kcc.com";
            decimal cartTotal = 0;
            decimal msrpTotal = 0;
            int itemTotalQuantity = 0;

            List<CartEntity> objLstCartDetail = userRepository.GetUserCartDetails(email);
            foreach (var item in objLstCartDetail)
            {
                cartTotal = cartTotal + item.UnitTotalCost ?? 0;
                msrpTotal = msrpTotal + item.MSRP ?? 0;
                itemTotalQuantity = itemTotalQuantity + item.Quantity ?? 0;
            }
            ViewBag.CartTotal = cartTotal;
            ViewBag.MsrpTotal = msrpTotal;
            ViewBag.ItemTotalQuantity = itemTotalQuantity;

            return PartialView("~/Views/Home/_UserCartDetails.cshtml", objLstCartDetail);
        }
        public ActionResult GetSavedItems(string id)
        {
            string email = "chandan.kumar@kcc.com";
            decimal cartTotal = 0;
            decimal msrpTotal = 0;
            int itemTotalQuantity = 0;

            List<CartEntity> objLstCartDetail = userRepository.GetUserSavedItems(email);
            foreach (var item in objLstCartDetail)
            {
                cartTotal = cartTotal + item.UnitTotalCost ?? 0;
                msrpTotal = msrpTotal + item.MSRP ?? 0;
                itemTotalQuantity = itemTotalQuantity + item.Quantity ?? 0;
            }
            ViewBag.CartTotal = cartTotal;
            ViewBag.MsrpTotal = msrpTotal;
            ViewBag.ItemTotalQuantity = itemTotalQuantity;

            return PartialView("~/Views/Home/_UserSavedItems.cshtml", objLstCartDetail);
        }
        public ActionResult RemoveFromCart(string cartId, string tabName)
        {
            bool status = false;
            if (tabName.ToLower().Equals("cart"))
                status = userRepository.DeleteFromCart(Convert.ToInt64(cartId));
            else
                status = userRepository.DeleteFromSavedItem(Convert.ToInt64(cartId));

            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClearCart(string userEmail, string tabName)
        {
            userEmail = "chandan.kumar@kcc.com";
            bool status = false;
            if (tabName.ToLower().Equals("cart"))
            {
                status = userRepository.ClearCart(userEmail);
            }
            else
            {
                status = userRepository.ClearSavedItem(userEmail);
            }

            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateItemQuantity(string Type, string CartId)
        {
            bool status = userRepository.UpdateItemQuantity(Type, Convert.ToInt64(CartId));

            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MoveToCart(string sku, string email)
        {
            bool status = userRepository.MoveFromSaveItemToCart(sku, email);

            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MoveToSavedItem(string id)
        {
            bool status = userRepository.MoveFromCartToSaveItem(Convert.ToInt64(id));

            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }

        #region CheckOut Information
        public ActionResult CheckOutPage()
        {

            string email = "chandan.kumar@kcc.com";
            decimal cartTotal = 0;
            decimal msrpTotal = 0;
            int itemTotalQuantity = 0;

            List<CartEntity> objLstCartDetail = userRepository.GetUserCartDetails(email);
            foreach (var item in objLstCartDetail)
            {
                cartTotal = cartTotal + item.UnitTotalCost ?? 0;
                msrpTotal = msrpTotal + item.MSRP ?? 0;
                itemTotalQuantity = itemTotalQuantity + item.Quantity ?? 0;
            }
            ViewBag.CartTotal = cartTotal;
            ViewBag.MsrpTotal = msrpTotal;
            ViewBag.ItemTotalQuantity = itemTotalQuantity;
            if (userRepository.getPaymentStatus() == 1)
                ViewBag.CoinPaymentmessage = "Coin payment successfull.";
            return View(objLstCartDetail);
        }
        public ActionResult GenerateOrders(string Id)
        {
            string orderId = string.Empty;
            bool status = userRepository.SaveCustomerOrders(Id, out orderId);
            CommonFunction.SetCookie("CartOrderId", orderId);
            return Json(new { Status = status, OrderId = orderId }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}