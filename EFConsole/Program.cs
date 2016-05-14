using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFConsole
{
    class Program
    {
        public class DepartmentCourse
        {
            public int  DepartmentID { get; set; }
            public string DepartmentName { get; set; }
            public string Title { get; set; }
        }


        static void Main(string[] args)
        {
            using (var db = new ContosoUniversityEntities())
            {
                //EF基本操作練習(db);

                //EF基本操作練習2(db);
            }
        }

        private static void EF基本操作練習2(ContosoUniversityEntities db)
        {
            #region 延遲載入

            //關閉代理物件(POCO Dynamic Proxy)
            db.Configuration.ProxyCreationEnabled = false;

            //延遲載入
            var one = db.Course.FirstOrDefault(m => m.DepartmentID == 1);

            Console.WriteLine(one.Title + "\t" + one.Department.Name);

            //非延遲載入
            foreach (var person in db.Person.Include("Department"))
            {
                Console.WriteLine(person.FirstName + person.LastName);

                foreach (var course in person.Course)
                {
                    Console.WriteLine("\t" + course.Title);
                }
            }

            #endregion

            #region  Native SQL

            var sql = "select a.DepartmentID, a.Name as DepartmentName, b.Title from Department as A left join Course as B on A.DepartmentID = B.DepartmentID";

            var data = db.Database.SqlQuery<DepartmentCourse>(sql);

            foreach (var item in data)
            {
                Console.WriteLine(item.DepartmentName + "\t" + item.Title);
            }

            #endregion

            #region Entity Framework with View

            var view1 = db.vwDepartmentCourse;

            foreach (var item in view1)
            {
                Console.WriteLine(item.DepartmentName + "\t" + item.Title);
            }

            #endregion

            #region EF default Value(with T4)

            var newData = new Course()
            {
                Title = "Test123",
                DepartmentID = 5
            };

            db.Course.Add(newData);
            db.SaveChanges();

            #endregion
        }

        private static void EF基本操作練習(ContosoUniversityEntities db)
        {
            #region 01.練習建立 Entity Framework 實體資料模型並取出 dbo.Course 資料

            //foreach (var item in db.Course)
            //{
            //    Console.WriteLine(item.Title);
            //}

            #endregion

            #region 03 練習用 LINQ 查詢資料 ( 查出 Course 資料表中所有 Git 資料 ) 與新增資料到資料庫 ( 查詢新增成功後 CourseID 的值 )

            //foreach (var item in db.Course.Where(m => m.Title.Contains("git")))
            //{
            //    Console.WriteLine(item.Title);
            //}

            db.Course.Add(new Course()
            {
                Title = "test1",
                Credits = 5,
                DepartmentID = 5
            });

            var newCourse = new Course()
            {
                Title = "test2",
                Credits = 5
            };

            newCourse.Department = db.Department.Find(1);

            db.Course.Add(newCourse);
            db.SaveChanges();

            #endregion

            #region 04 練習用 Entity Framework 批次更新與刪除資料

            foreach (var item in db.Course)
            {
                Console.WriteLine(item.Title);
            }

            var courses = db.Course.Where(m => m.CourseID > 7);

            db.Course.RemoveRange(courses);
            db.SaveChanges();

            foreach (var item in db.Course)
            {
                Console.WriteLine(item.Title);
            }

            #endregion
        }
    }
}
