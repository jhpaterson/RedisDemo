using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace RedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // to browse database use Redis Commander
            // node application, installed with npm install -g redis-commander
            // installs in C:\Users\jim\AppData\Roaming\npm
            // run with redis-commander
            // did this in node_modules folder inside node installation in Program Files
            // redis-commander listens on port 8081

            var redisClient = new RedisClient("localhost",6379);   // default port

            using (var cars = redisClient.As<Car>())
            {
                if (cars.GetAll().Count > 0)
                    cars.DeleteAll();

                var dansFord = new Car
                {
                    Id = cars.GetNextSequence(),
                    Title = "Dan's Ford",
                    Make = new Make { Name = "Ford" },
                    Model = new Model { Name = "Fiesta" }
                };
                var beccisFord = new Car
                {
                    Id = cars.GetNextSequence(),
                    Title = "Becci's Ford",
                    Make = new Make { Name = "Ford" },
                    Model = new Model { Name = "Focus" }
                };
                var vauxhallAstra = new Car
                {
                    Id = cars.GetNextSequence(),
                    Title = "Dans Vauxhall Astra",
                    Make = new Make { Name = "Vauxhall" },
                    Model = new Model { Name = "Asta" }
                };
                var vauxhallNova = new Car
                {
                    Id = cars.GetNextSequence(),
                    Title = "Dans Vauxhall Nova",
                    Make = new Make { Name = "Vauxhall" },
                    Model = new Model { Name = "Nova" }
                };

                var carsToStore = new List<Car> { dansFord, beccisFord, vauxhallAstra, vauxhallNova };
                cars.StoreAll(carsToStore);

                Console.WriteLine("Redis Has-> " + cars.GetAll().Count + " cars");
                Console.WriteLine(cars.GetAll().Dump());

                cars.ExpireAt(vauxhallAstra.Id, DateTime.Now.AddSeconds(5)); //Expire Vauxhall Astra in 5 seconds

                Thread.Sleep(6000); //Wait 6 seconds to prove we can expire our old Astra

                Console.WriteLine("Redis Has-> " + cars.GetAll().Count + " cars");
                Console.WriteLine(cars.GetAll().Dump());

                //Get Cars out of Redis
                var carsFromRedis = cars.GetAll().Where(car => car.Make.Name == "Ford");

                foreach (var car in carsFromRedis)
                {
                    Console.WriteLine("Redis Has a ->" + car.Title);
                }

            }
            Console.ReadLine();
        }
    }

    public class Car
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Make Make { get; set; }
        public Model Model { get; set; }
    }

    public class Make
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Model
    {
        public int Id { get; set; }
        public Make Make { get; set; }
        public string Name { get; set; }
    }
}
