using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var DbUtils = new MongoDBUtils("mongodb://sa:123456@localhost:27017/MyTest");
            var user = DbUtils.Query<User>(t => t.Id != null)?.FirstOrDefault();
            if (user == null)
            {
                DbUtils.InsertOne(new User()
                {
                    Email = "123456@qq.com",
                    UserName = "zhangsan",
                    Password = "123456",
                });
            }

        }
    }
}
