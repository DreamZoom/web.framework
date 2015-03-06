using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class User : dz.web.model.ModelBase
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public string Job { get; set; }
    }


    class UserService : dz.web.service.ServiceBase
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            UserService us = new UserService();
           // us.Insert(new User() { UserName="王亚军", Address="沐川", Age=11, Job="学生", Phone="123456789" });
            var u = us.GetModel("ID=3") as User;
            u.UserName = "王小龙";
            Console.WriteLine(us.Update(u));
            Console.ReadKey();
        }
    }
}
