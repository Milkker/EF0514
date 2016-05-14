using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ContosoUniversityEntities())
            {
               //EF基本操作練習(db);
            }

            Console.ReadLine();
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
