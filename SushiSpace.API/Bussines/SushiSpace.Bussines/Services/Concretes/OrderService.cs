using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Concretes
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
       
        private readonly IProductServices _productServices;
        private readonly IMapper mapper;
        private readonly UserManager<User> _userManager;

        private readonly IOrderProductRepository _orderProductRepository;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, UserManager<User> userManager, IOrderProductRepository orderProductRepository, IProductServices productServices)
        {
            _orderRepository = orderRepository;
            this.mapper = mapper;
            _userManager = userManager;
            _orderProductRepository = orderProductRepository;
            _productServices = productServices;
        }

        public async Task<Order> CreateOrder(OrderDTO orderdto)
        {
            if (string.IsNullOrEmpty(orderdto.UserId))
            {
                throw new ArgumentNullException(nameof(orderdto.UserId));
            }
            if (string.IsNullOrEmpty(orderdto.Phone))
            {
                throw new ArgumentNullException(nameof(orderdto.UserId));
            }
            if (string.IsNullOrEmpty(orderdto.Adress))
            {
                throw new ArgumentNullException(nameof(orderdto.UserId));
            }
            var order = mapper.Map<Order>(orderdto);
            order.User= await _userManager.FindByIdAsync(orderdto.UserId);
            order.CreateTime = DateTime.Now;
            var result = await _orderRepository.Create(order);
            return result;
            
        }

        public async Task<List<OrderProduct>> CreateOrderProducts(List<OrderProduct> orderProducts)
        {
            foreach (OrderProduct item in orderProducts)
            {
                item.CreateTime = DateTime.Now;

                // Ensure ProductId is valid
                if (item.ProductId <= 0)
                {
                    throw new ArgumentException("Invalid ProductId.");
                }

                // Fetch product details
                var product = await _productServices.GetProduct(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                }

                // Assign fetched product to OrderProduct
                item.Products = product;

                // Create OrderProduct in repository
                var result = await _orderProductRepository.Create(item);
                if (result == null)
                {
                    throw new Exception("Failed to create order product.");
                }
            }

            return orderProducts;
        }


        public async Task<bool> DeleteOrder(int orderid)
        {
            if (orderid<=0) throw new ArgumentNullException(nameof(orderid));
            var result= await _orderRepository.Delete(orderid);
            if (!result)
            {
                throw new Exception("can not delete order");
            }
            return  result;
            
        }

        public async Task<IQueryable<Order>> GetAll()
        {
            IQueryable<Order> order=await _orderRepository.GetAll(null,null,"User","Products");
            return order;
        }

        public async Task<IQueryable<Order>> GetAllByUser(string userid)
        {
            IQueryable<Order> order=await _orderRepository.GetAll(null,x=>x.User.Id==userid,"User","Products");
            return order;
        }

        public async Task<OrderDTO> GetOrder(int orderid)
        {
            var order = await _orderRepository.GetEntity(orderid.ToString(), null, null, "User", "Products");
            var dto = mapper.Map<OrderDTO>(order);
            return dto;
        }

        public async Task<Order> UpdateOrder(OrderDTO neworder, int orderid)
        {
          
            Order oldOrder = await _orderRepository.GetEntity(orderid.ToString(), null, null, "User", "Products");
   
            if (oldOrder == null)
            {
                throw new Exception("Order not found");
            }
            mapper.Map(neworder, oldOrder);

            // Сохранение изменений в репозитории
            await _orderRepository.SaveChangesAsync();

            return oldOrder;
        }
    }
}
