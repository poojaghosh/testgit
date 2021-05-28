using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCPAdvantageCart.Models.AdvantageBO;
using BlockChainPayment.Model.Entity;
using BlockChainPayment.Models.AdvantageBO;

namespace KCPAdvantageCart.Models.AdvantageBL
{
    public class UserRepository
    {
        private KCPAdvanatgeEntities context;
        private KCPAdvanatgeEntities writeContext;
        public static int paymentFlag = 0;
        public UserRepository()
        {
            this.context = new KCPAdvanatgeEntities();
            this.writeContext = new KCPAdvanatgeEntities();
        }

        public List<CartEntity> GetUserCartDetails(string userEmail)
        {
            List<CartEntity> objUserCartDetails = new List<CartEntity>();
            objUserCartDetails = (from uc in context.UserCarts
                                  where uc.UserEmail == userEmail && uc.Active == true
                                  select new CartEntity
                                  {
                                      Id = uc.Id,
                                      UserEmail = uc.UserEmail,
                                      ProductName = uc.ProductName,
                                      ProductDescription = uc.ProductDescription,
                                      ProductSKU = uc.ProductSKU,
                                      Quantity = uc.Quantity,
                                      UnitRate = uc.UnitRate,
                                      MSRP = uc.Quantity * uc.UnitRate,
                                      UnitTotalCost = (uc.Quantity * uc.UnitRate) - (((uc.Quantity * uc.UnitRate) * uc.Discount) / 100),
                                      Discount = uc.Discount,
                                      CreatedDate = uc.CreatedDate
                                  }
                                  ).ToList();
            return objUserCartDetails;
        }
        public List<CartEntity> GetUserSavedItems(string userEmail)
        {
            List<CartEntity> objUserCartDetails = new List<CartEntity>();
            objUserCartDetails = (from uc in context.UserSavedItems
                                  where uc.UserEmail == userEmail && uc.Active == true
                                  select new CartEntity
                                  {
                                      Id = uc.Id,
                                      UserEmail = uc.UserEmail,
                                      ProductName = uc.ProductName,
                                      ProductDescription = uc.ProductDescription,
                                      ProductSKU = uc.ProductSKU,
                                      Quantity = uc.Quantity,
                                      UnitRate = uc.UnitRate,
                                      MSRP = uc.Quantity * uc.UnitRate,
                                      UnitTotalCost = (uc.Quantity * uc.UnitRate) - (((uc.Quantity * uc.UnitRate) * uc.Discount) / 100),
                                      Discount = uc.Discount,
                                      CreatedDate = uc.CreatedDate
                                  }
                                  ).ToList();
            return objUserCartDetails;
        }
        public bool DeleteFromCart(long id)
        {
            bool status = false;
            try
            {
                UserCart objCart = context.UserCarts.Where(x => x.Id == id).FirstOrDefault();
                if (objCart != null)
                {
                    objCart.Active = false;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }

            return status;
        }
        public bool DeleteFromSavedItem(long id)
        {
            bool status = false;
            try
            {
                UserSavedItem objSavedItem = context.UserSavedItems.Where(x => x.Id == id).FirstOrDefault();
                if (objSavedItem != null)
                {
                    context.UserSavedItems.Remove(objSavedItem);
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }

            return status;
        }

        public bool ClearCart(string userEmail)
        {
            bool status = false;
            try
            {
                var objCartDetails = context.UserCarts.Where(x => x.UserEmail == userEmail);
                foreach (var item in objCartDetails)
                {
                    UserCart objCart = writeContext.UserCarts.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (objCart != null)
                    {
                        objCart.Active = false;
                        writeContext.SaveChanges();
                        status = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return status;
        }
        public bool ClearSavedItem(string userEmail)
        {
            bool status = false;
            try
            {
                var objSavedItem = context.UserSavedItems.Where(x => x.UserEmail == userEmail);
                foreach (var item in objSavedItem)
                {
                    UserSavedItem objSaved = writeContext.UserSavedItems.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (objSaved != null)
                    {
                        writeContext.UserSavedItems.Remove(objSaved);
                        writeContext.SaveChanges();
                        status = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return status;
        }
        public bool UpdateItemQuantity(string Type, long id)
        {
            bool status = false;
            try
            {
                UserCart objCart = context.UserCarts.Where(x => x.Id == id).FirstOrDefault();
                if (objCart != null)
                {
                    if (Type.Trim().ToLower() == "inc")
                        objCart.Quantity = objCart.Quantity + 1;
                    if (Type.Trim().ToLower() == "des")
                        objCart.Quantity = objCart.Quantity - 1;

                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public bool MoveFromCartToSaveItem(long id)
        {
            bool status = false;
            try
            {
                UserCart objCart = context.UserCarts.Where(x => x.Id == id).FirstOrDefault();
                if (objCart != null)
                {
                    UserSavedItem objSavedItem = new UserSavedItem();
                    objSavedItem.ProductName = objCart.ProductName;
                    objSavedItem.ProductSKU = objCart.ProductSKU;
                    objSavedItem.ProductDescription = objCart.ProductDescription;
                    objSavedItem.UserId = objCart.UserId;
                    objSavedItem.UserEmail = objCart.UserEmail;
                    objSavedItem.Quantity = objCart.Quantity;
                    objSavedItem.MSRP = objCart.MSRP;
                    objSavedItem.UnitRate = objCart.UnitRate;
                    objSavedItem.Discount = objCart.Discount;
                    objSavedItem.Active = true;

                    writeContext.UserSavedItems.Add(objSavedItem);
                    writeContext.SaveChanges();

                    objCart.Active = false;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public bool MoveFromSaveItemToCart(string sku, string email)
        {
            bool status = false;
            try
            {
                UserCart objCart = context.UserCarts.Where(x => x.ProductSKU == sku && x.UserEmail == email).FirstOrDefault();
                if (objCart != null)
                {
                    objCart.Active = true;
                    context.SaveChanges();

                    UserSavedItem objSavedItem = writeContext.UserSavedItems.Where(x => x.ProductSKU == sku && x.UserEmail == email).FirstOrDefault();
                    if (objSavedItem != null)
                    {
                        writeContext.UserSavedItems.Remove(objSavedItem);
                        writeContext.SaveChanges();
                    }
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public void UpdatePaymentStatus(int flag)
        {
          
            try
            {
                paymentFlag = flag;
              
            }
            catch (Exception ex)
            {

            }
         
        }
        public int getPaymentStatus()
        {
            return paymentFlag;
        }

        #region Customer Order Information

        /// <summary>
        /// SaveCustomerOrders - This method used to generate the order and return the order id
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool SaveCustomerOrders(string userEmail, out string orderId)
        {
            bool flag = false;
            orderId = string.Empty;
            try
            {
                List<CartEntity> cartInfo = new List<CartEntity>();
                cartInfo = (from uc in context.UserCarts
                            where uc.UserEmail == userEmail && uc.Active == true
                            select new CartEntity
                            {
                                Id = uc.Id,
                                UserEmail = uc.UserEmail,
                                ProductName = uc.ProductName,
                                ProductDescription = uc.ProductDescription,
                                ProductSKU = uc.ProductSKU,
                                Quantity = uc.Quantity,
                                UnitRate = uc.UnitRate,
                                MSRP = uc.Quantity * uc.UnitRate,
                                UnitTotalCost = (uc.Quantity * uc.UnitRate) - (((uc.Quantity * uc.UnitRate) * uc.Discount) / 100),
                                Discount = uc.Discount,
                                CreatedDate = uc.CreatedDate
                            }
                                  ).ToList();

                decimal cartTotal = 0;
                int itemTotalQuantity = 0;

                foreach (var item in cartInfo)
                {
                    cartTotal = cartTotal + item.UnitTotalCost ?? 0;
                    itemTotalQuantity = itemTotalQuantity + item.Quantity ?? 0;
                }
                //var cartInfo = context.UserCarts.Where(x => x.UserEmail == userEmail && x.Active == true).ToList();
                if (cartInfo != null)
                {
                    CustomerOrder _customerOrder = new CustomerOrder();
                    string _orderId = Convert.ToString(Guid.NewGuid().ToString("N").Substring(0, 12)).ToUpper();
                    _customerOrder.OrderId = _orderId;
                    _customerOrder.UserEmail = userEmail;
                    _customerOrder.UserId = 1;
                    _customerOrder.TotalCost = cartTotal;
                    _customerOrder.TotalQuantity = itemTotalQuantity;
                    _customerOrder.PaymentStatus = false;
                    _customerOrder.CreatedDate = DateTime.UtcNow;
                    _customerOrder.Active = true;
                    writeContext.CustomerOrders.Add(_customerOrder);
                    writeContext.SaveChanges();

                    foreach (var item in cartInfo)
                    {
                        CustomerOrderDetail _customerOrderDetails = new CustomerOrderDetail();
                        _customerOrderDetails.OrderId = _orderId;
                        _customerOrderDetails.ProductName = item.ProductName;
                        _customerOrderDetails.ProductSKU = item.ProductSKU;
                        _customerOrderDetails.ProductDescription = item.ProductDescription;
                        _customerOrderDetails.Quantity = item.Quantity;
                        _customerOrderDetails.MSRP = item.MSRP;
                        _customerOrderDetails.UnitRate = item.UnitRate;
                        _customerOrderDetails.Discount = item.Discount;
                        _customerOrderDetails.CreatedDate = DateTime.UtcNow;
                        _customerOrderDetails.Active = true;
                        writeContext.CustomerOrderDetails.Add(_customerOrderDetails);
                        writeContext.SaveChanges();
                    }
                    flag = true;

                    orderId = _orderId;
                }

            }
            catch (Exception)
            {

            }
            return flag;
        }

        /// <summary>
        /// GetCustomerOrderInfo - This method used to get the order information from datatbase
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OrderEntity GetCustomerOrderInfo(string userEmail, string orderId)
        {
            var orderInfo = context.CustomerOrders.FirstOrDefault(x => x.OrderId == orderId && x.UserEmail == userEmail);
            OrderEntity _order = new OrderEntity();
            _order.OrderId = orderInfo.OrderId;
            _order.TotalCost = orderInfo.TotalCost;
            _order.PaidTransactionVia = Convert.ToString(orderInfo.PaidTransactionVia);
            return _order;
        }

        /// <summary>
        /// UpdateCutomerOrderPaymentStatus - This method used to update the payment status against the order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool UpdateCutomerOrderPaymentStatus(string orderId, string paymentVia)
        {
            bool status = false;
            try
            {
                var orderInfo = context.CustomerOrders.FirstOrDefault(x => x.OrderId == orderId);
                if (orderInfo != null)
                {
                    orderInfo.PaymentStatus = true;
                    orderInfo.PaidTransactionVia = paymentVia;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }

            return status;
        }

        #endregion
    }
}