using TestTask.Services.Interfaces;
using TestTask.Data;
using TestTask.Models;
using TestTask.Enums;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace TestTask
{

    public class UserService : IUserService
    {
        private ApplicationDbContext dataBase;
        public UserService(ApplicationDbContext dataBase)
        {
            this.dataBase = dataBase;
        }

        public Task<User> GetUser() {

            User largestOrderUser = null;

            var largestOrder = (from order in this.dataBase.Orders
                         group order by order.UserId into Users
                         orderby Users.Count() descending
                         select Users.FirstOrDefault()).ToList();

            if (largestOrder.Count != 0)
            {

                largestOrderUser = (from user in this.dataBase.Users
                                    where user.Id == largestOrder.First().UserId
                                    select user).FirstOrDefault();
                largestOrderUser.Orders = largestOrder;
            }
            else
            {
                largestOrderUser = (from user in this.dataBase.Users 
                                    select user).FirstOrDefault();
            }
            
            return Task.FromResult(largestOrderUser);
        }

        public Task<List<User>> GetUsers()
        {
            var inactiveUsers = (from user in this.dataBase.Users
                         where user.Status == UserStatus.Inactive
                         select user).ToList();
            foreach (var user in inactiveUsers)
            {
                var orders = (from order in this.dataBase.Orders
                              where order.UserId == user.Id
                              select order).ToList();
                user.Orders = orders;
            }
            return Task.FromResult(inactiveUsers);
        }
    }

}