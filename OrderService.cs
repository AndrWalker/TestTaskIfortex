using TestTask.Services.Interfaces;
using TestTask.Data;
using TestTask.Models;
using System.Collections.Generic;

namespace TestTask
{

    public class OrderService : IOrderService
    {
        private ApplicationDbContext dataBase;
        public OrderService(ApplicationDbContext dataBase)
        {
            this.dataBase = dataBase;
        }

        public Task<Order> GetOrder()
        {
            var highCostOrder = (from order in this.dataBase.Orders
                         orderby (order.Quantity * order.Price) descending
                         select order).FirstOrDefault();
            
            return Task.FromResult(highCostOrder);
        }

        public Task<List<Order>> GetOrders()
        {
            var quantityMore10Order = (from order in this.dataBase.Orders
                                 where order.Quantity > 10
                                 select order).ToList();
            foreach (var order in quantityMore10Order)
                order.User = (from user in this.dataBase.Users
                               where user.Id == order.UserId
                               select user).First();

            return Task.FromResult(quantityMore10Order);
        }
    }

}
