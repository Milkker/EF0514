using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("APP 2");

            using (var db = new ContosoUniversityEntities())
            {
                db.Database.Log = Console.WriteLine;
                var c = db.Department.Find(1);

                c.Name = "??";

                Console.ReadLine();

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("資料已被其他使用者異動。");
                }
            }
        }
    }
}
