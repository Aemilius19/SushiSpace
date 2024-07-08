using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using Stripe;
using Stripe.Climate;
using SushiSpace.Web.Models.DTOs;
using SushiSpace.Web.Models.Entites;
using SushiSpace.Web.Models.ViewModels;
using System.Linq;
using System.Net;

namespace SushiSpace.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index(List<CartProduct> products)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must first log in to proceed with the order";
                return RedirectToAction("Index", "Cart");
            }

            List<OrderProduct> orders = new List<OrderProduct>();
            foreach (var product in products)
            {
                OrderProduct orderProduct = new OrderProduct()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,

                    Quantity = product.Quantity,
                    Price = product.Price * product.Quantity// Ensure Quantity is assigned
                };
                //if (orders.Contains(orderProduct))
                //{
                //    orderProduct.Quantity= product.Quantity;
                //}
                orders.Add(orderProduct);
            }
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Address, string Email, string Phone, List<OrderProduct> Products)
        {
            // Проверяем, что пользователь аутентифицирован
            if (User.Identity.IsAuthenticated)
            {
                // Проверяем наличие утверждения EmailConfirmed со значением "true"
                var emailConfirmedClaim = User.FindFirst("EmailConfirmed");
                if (emailConfirmedClaim != null && emailConfirmedClaim.Value == "True")
                {
                    var response = await _httpClient.GetAsync($"http://localhost:5119/api/User/username/{User.Identity.Name}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(content);
                    OrderDTO order = new OrderDTO()
                    {
                        Adress = Address,
                        Email = Email,
                        Phone = Phone,
                        PaymentStatus = "Waiting",
                        Products = null,
                        UserId = user.Id
                    };

                    var Jsondto = JsonConvert.SerializeObject(order);
                    var contents = new StringContent(Jsondto, System.Text.Encoding.UTF8, "application/json");
                    var responses = await _httpClient.PostAsync($"http://localhost:5119/api/Order", contents);
                    
                    responses.EnsureSuccessStatusCode();
                    var orders=await responses.Content.ReadAsStringAsync();
                    

                    // Утверждение EmailConfirmed есть и равно "true"
                    TempData["Address"] = Address;
                    TempData["Email"] = Email;
                    TempData["Phone"] = Phone;
                    TempData["Total"] = Products.Sum(p => p.Price * p.Quantity).ToString("0.00");
                    TempData["Order"] = JsonConvert.SerializeObject(order);
                    TempData["OrderId"] = orders;
                    TempData["Products"] = JsonConvert.SerializeObject(Products);

                    return RedirectToAction("ConfirmPayment", "Order"); // Возвращаем представление подтверждения заказа
                }
            }

            // Если утверждение не найдено или не равно "true", перенаправляем пользователя
            TempData["Message"] = "To make an order, you need to confirm your email.";
            return RedirectToAction("Index", "Cart");
        }
        [HttpGet]
        public IActionResult ConfirmPayment()
        {
            // Получение данных из TempData
            var orderssJson = TempData["OrderId"] as string; 
            
            var address = TempData["Address"] as string;
            var email = TempData["Email"] as string;
            var phone = TempData["Phone"] as string;
            var productsJson = TempData["Products"] as string;
            var products = JsonConvert.DeserializeObject<List<OrderProduct>>(productsJson);
            var total = TempData["Total"] as string;
            var ordersJson = TempData["Order"] as string;
            var order = JsonConvert.DeserializeObject<Order>(ordersJson);

            // Создание ViewModel для передачи в представление
            var viewModel = new ConfirmPaymentViewModel
            {
                Address = address,
                Email = email,
                Phone = phone,
                Products = products,
                Total = total
            };
            TempData["Address"] = address;
            TempData["Email"] = email;
            TempData["Phone"] = phone;
            TempData["Total"] = total;
            TempData["Order"] = JsonConvert.SerializeObject(order);

            TempData["Products"] = JsonConvert.SerializeObject(products);
            TempData["OrderId"] = orderssJson;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(string stripeEmail, string stripeToken)
        {
            try
            {
                // Десериализация списка продуктов из TempData
                var address = TempData["Address"] as string;
                var email = TempData["Email"] as string;
                var phone = TempData["Phone"] as string;
                var productsJson = TempData["Products"] as string;
                var products = JsonConvert.DeserializeObject<List<OrderProduct>>(productsJson);
                var total = TempData["Total"] as string;
                var ordersJson = TempData["Order"] as string;
                var order = JsonConvert.DeserializeObject<Order>(ordersJson);
                
                var orderssJson = TempData["OrderId"] as string;
                var orderss = JsonConvert.DeserializeObject<Order>(orderssJson);

                // Создание объекта Customer в Stripe
                var optionsCust = new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    Name = User.Identity.Name,
                    Phone = phone
                };
                var serviceCust = new CustomerService();
                Customer customer = serviceCust.Create(optionsCust);

                // Конвертация суммы в копейки (центы) для Stripe
                var totalInCents = (int)(decimal.Parse(total) * 100);

                // Создание объекта Charge в Stripe
                var optionsCharge = new ChargeCreateOptions
                {
                    Amount = (long)totalInCents,
                    Currency = "USD",
                    Description = "Product Selling amount",
                    Source = stripeToken,
                    ReceiptEmail = stripeEmail
                };
                var serviceCharge = new ChargeService();
                Charge charge = serviceCharge.Create(optionsCharge);

                // Проверка успешности платежа
                if (charge.Status != "succeeded")
                {
                    // В случае неудачи возвращаем представление с ошибкой
                    ModelState.AddModelError("Address", "There was a problem with the payment.");
                    return View(products);
                }
                var response = await _httpClient.GetAsync($"http://localhost:5119/api/User/username/{User.Identity.Name}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(content);
                // Обновление информации о заказе
                OrderDTO ordernew = new OrderDTO()
                {
                    Adress = address,
                    Email = email,
                    Phone = phone,
                    PaymentStatus = "Paid",
                    Products = products,
                    UserId = user.Id  // Предполагаем, что UserId соответствует имени пользователя в данном случае
                };

                var jsonordernew = JsonConvert.SerializeObject(ordernew);
                var contentnew = new StringContent(jsonordernew, System.Text.Encoding.UTF8, "application/json");

                var responseput = await _httpClient.PutAsync($"http://localhost:5119/api/Order/{orderss.Id}", contentnew);
                responseput.EnsureSuccessStatusCode();

                // Возвращаем редирект на Index с параметром, указывающим на успешность операции
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если что-то пошло не так
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View();
            }
        }

    }
}
