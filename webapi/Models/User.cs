using System;
using System.Collections.Generic;
using webapi.Attributes;

namespace webapi.Models
{
    public class User
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }


        public List<User> createUserTest()
        {

            var Customer = new User();
            var Supplier = new User();

            Customer.Id = 1;
            Customer.Email = "customer@test.com";
            Customer.FirstName = "Customer FirstName";
            Customer.LastName = "Customer LastName";
            Customer.RoleId = (int)AuthorizationRole.Customer;
            Customer.TypeId = (int)AuthorizationRole.Customer;

            Supplier.Id = 2;
            Supplier.Email = "supplier@test.com";
            Supplier.FirstName = "Supplier FirstName";
            Supplier.LastName = "Supplier LastName";
            Supplier.RoleId = (int)AuthorizationRole.Supplier;
            Supplier.TypeId = (int)AuthorizationRole.Supplier;

            List<User> ttt = new List<User>();
            ttt.Add(Customer);
            ttt.Add(Supplier);

            return ttt;
        }

    }


}
