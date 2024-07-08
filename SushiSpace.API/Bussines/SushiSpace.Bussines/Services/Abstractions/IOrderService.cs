using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Abstractions
{
    public interface IOrderService
    {

        Task<IQueryable<Order>> GetAll();

        Task<IQueryable<Order>> GetAllByUser(string userid);

        Task<OrderDTO> GetOrder(int orderid);

        Task<Order> CreateOrder(OrderDTO order);

        Task<Order> UpdateOrder(OrderDTO neworder, int orderid);
        Task<bool> DeleteOrder(int orderid);

        Task<List<OrderProduct>> CreateOrderProducts(List<OrderProduct> orderProducts);
    }
}
