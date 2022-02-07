using BookShopApp.data.Concrete.EfCore;
using BookShopApp.entity;
using BookShopApp.webui.Identity;
using BookShopApp.webui.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Controllers
{

    [Authorize]//sepete ekle için mutlaka kayıtlı bır kullanıcı olsun kuralı
    public class CartController : Controller
    {
        private EfCoreCartRepository _cartService;
        private EfCoreOrderRepository _orderService;
        private EfCoreProductRepository _productService;
        private UserManager<User> _userManager;

        public CartController(UserManager<User> userManager)
        {
            this._orderService = new EfCoreOrderRepository();
            this._cartService = new EfCoreCartRepository();
            this._productService = new EfCoreProductRepository();
            this._userManager = userManager;
        }

        public IActionResult Index()
        {//başlangıcta kartı bize getirecek yapı]
            var cart = _cartService.GetByUserId(_userManager.GetUserId(User));
            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    KitapAdi = i.Product.KitapAdi,
                    Ucret = (double?)i.Product.Ucret,
                    ResimUrl = i.Product.ResimUrl,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        public void AddCart(string userId, int productId, int quantity)
        {
            var cart = _cartService.GetByUserId(userId);
            if (cart != null)
            {//1.durum ->eklenmek isteyen ürün sepette var mı(güncelleme)
             //2.durum ->eklenmek istenen ürün sepette var ve yeni kayıt oluştur.(kayıt ekleme)

                var index = cart.CartItems.FindIndex(i => i.ProductId == productId);

                if (index < 0)
                {
                    cart.CartItems.Add(new CartItem()
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        CartId = cart.Id
                    });
                }
                else
                {
                    cart.CartItems[index].Quantity += quantity;
                }
                _cartService.Update(cart);
            }
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            AddCart(userId, productId, quantity);
            return RedirectToAction("Index");

        }

        public void DeleteCart(string userId, int productId)
        {
            var cart = _cartService.GetByUserId(userId);
            if (cart != null)
            {
                _cartService.DeleteFromCart(cart.Id, productId);
            }
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            DeleteCart(userId, productId);
            return RedirectToAction("Index");

        }


        public IActionResult Checkout()
        {
            var cart = _cartService.GetByUserId(_userManager.GetUserId(User));
            var orderModel = new OrderModel();


            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    KitapAdi = i.Product.KitapAdi,
                    Ucret = i.Product.Ucret,
                    ResimUrl = i.Product.ResimUrl,
                    Quantity = i.Quantity
                }).ToList()
            };
            return View(orderModel);
        }

        [HttpPost]
        public IActionResult Checkout(OrderModel model)
        {
            if(ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cart = _cartService.GetByUserId(_userManager.GetUserId(User));

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductId = i.ProductId,
                        KitapAdi = i.Product.KitapAdi,
                        Ucret = (double)i.Product.Ucret,
                        ResimUrl = i.Product.ResimUrl,
                        Quantity = i.Quantity
                    }).ToList()
                };

                var payment = PaymentProcess(model);

                if (payment.Status=="success")
                {
                    SaveOrder(model, payment, userId);
                    ClearCart(model.CartModel.CartId);
                    return Redirect("/orders");
                }
                else
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{payment.ErrorCode}",
                        AlertType = "danger"
                    };

                    TempData["message"] = JsonConvert.SerializeObject(msg);
                }
            }       
            return View(model);                
        }

        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order();
            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = EnumOrderState.completed;
            order.PaymentType = EnumPaymentType.CreditCard;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderDate = new DateTime();
            order.FirstName = model.FirstName;
            order.LastName = model.LastName;
            order.UserId = userId;
            order.Address = model.Address;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.City = model.City;
            order.Note = model.Note;

            order.OrderItems = new List<entity.OrderItem>();

            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new entity.OrderItem()
                {
                    Ucret = item.Ucret,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };

                _productService.EditQuantity(item.ProductId, item.Quantity);

                order.OrderItems.Add(orderItem);
            }
            _orderService.CreateOrder(order);

        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-eBxYQClWDMvRxnoPWqNfH75l0IdmwYHE";
            options.SecretKey = "sandbox-PLUyMJRRcx9ms3zI1Jkk6GbNa3apBm4I";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";


            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111, 999999999).ToString();
            request.Price = model.CartModel.TotalPrice().ToString().Replace(",", ".");
            request.PaidPrice = model.CartModel.TotalPrice().ToString().Replace(",", ".");
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            BasketItem basketItem;

            foreach (var item in model.CartModel.CartItems)
            {
                var sum = item.Ucret * item.Quantity;
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.KitapAdi;
                basketItem.Category1 = "Şiir";
                basketItem.Price = sum.ToString().Replace(",",".");
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;
                           
            return Payment.Create(request, options);
        }
    
        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = _orderService.GetOrders(userId);

            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;
            foreach (var order in orders )
            {
                orderModel = new OrderListModel();

                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.Phone = order.Phone;
                orderModel.FirstName = order.FirstName;
                orderModel.LastName = order.LastName;
                orderModel.Email = order.Email;
                orderModel.Address = order.Address;
                orderModel.City = order.City;
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentType = order.PaymentType;

                orderModel.OrderItems = order.OrderItems.Select(i => new OrderItemModel()
                {
                    
                    OrderItemId = i.Id,
                    Name = i.Product.KitapAdi,
                    Ucret = (double)i.Ucret,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ResimUrl

                }).ToList();

                orderListModel.Add(orderModel);
            }
            orderListModel.Reverse();
            return View("Orders", orderListModel);
        }
        
       
    }
}