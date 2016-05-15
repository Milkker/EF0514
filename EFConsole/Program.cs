using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;

namespace EFConsole
{
    class Program
    {
        public class DepartmentCourse
        {
            public int DepartmentID { get; set; }
            public string DepartmentName { get; set; }
            public string Title { get; set; }
        }


        static void Main(string[] args)
        {
            using (var db = new ContosoUniversityEntities())
            {
                db.Database.Log = (msg) =>
                {
                    File.AppendAllText(@"D:\log.txt", msg);
                };

                //EF基本操作練習(db);

                //EF基本操作練習2(db);

                //EF類別介紹(db);

                //預存程序(db);

                //使用列舉型別(db);

                #region 預先載入

                //取消延遲載入
                db.Configuration.LazyLoadingEnabled = false;

                //延遲載入
                //var course1 = db.Course.Where(p => p.CourseType.HasFlag(CourseType.前端));

                //foreach (var item in course1)
                //{
                //    Console.WriteLine(item.Title + "\t" + item.Department.Name);
                //}

                //預先載入
                //var course2 = db.Course.Include(m => m.Department)
                //    .Where(p => p.CourseType.HasFlag(CourseType.前端));

                //foreach (var item in course2)
                //{
                //    Console.WriteLine(item.Title + "\t" + item.Department.Name);
                //}

                //動態
                var course3 = db.Course
                    .Where(p => p.CourseType.HasFlag(CourseType.前端));

                foreach (var item in course3)
                {
                    var refLink = db.Entry(item).Reference(p => p.Department);
                    if (!refLink.IsLoaded)
                    {
                        refLink.Load();
                    }
                    Console.WriteLine(item.Title + "\t" + item.Department.Name);
                }

                #endregion
            }

            //離線模式資料操作();
        }

        private static void 使用列舉型別(ContosoUniversityEntities db)
        {
            var c = db.Course.Find(1);

            c.CourseType = CourseType.前端;

            c = db.Course.Find(2);

            c.CourseType = CourseType.前端 | CourseType.後端;

            c = db.Course.Find(3);

            c.CourseType = CourseType.前端 | CourseType.後端 | CourseType.全端;

            db.SaveChanges();

            foreach (var item in db.Course.Where(m => m.CourseType.HasFlag(CourseType.前端)))
            {
                Console.WriteLine(item.Title + "\t" + item.CourseType);
            }
        }

        private static void 預存程序(ContosoUniversityEntities db)
        {
            //Read
            var data = db.Get部門名稱與課程數量統計(3);

            foreach (var item in data)
            {
                Console.WriteLine(item.DepartmentID + "\t" + item.Name + "\t" + item.CourseCount);
            }

            //Create
            //至Edmx檢視【對應詳細資料】-> 【將實體對應到函式】設定新增時的預存程序。
            db.Department.Add(new Department()
            {
                Name = "測試",
                Budget = 123.45m,
                InstructorID = 5//因為預存程序並未處理此欄位，
            });
            db.SaveChanges();
        }

        private static void 離線模式資料操作()
        {
            #region EF 離線模式(Attach)

            var c = new Course()
            {
                CourseID = 7,
                Title = "123",
                DepartmentID = 1,
                Credits = 1
            };

            using (var db = new ContosoUniversityEntities())
            {
                c.Title = "Test1";
                Console.WriteLine(db.Entry(c).State);
                db.SaveChanges();

                db.Course.Attach(c);
                Console.WriteLine(db.Entry(c).State);

                c.Title = "Test2";
                Console.WriteLine(db.Entry(c).State);

                db.SaveChanges();

                //EF cache Sample
                var cacheTest = new Course()
                {
                    CourseID = 6,
                    Title = "Cache1",
                    DepartmentID = 1,
                    Credits = 1
                };

            }

            using (var db = new ContosoUniversityEntities())
            {
                c.Title = "Test3";
                db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            #endregion
        }

        private static void EF類別介紹(ContosoUniversityEntities db)
        {
            #region DbSet<T>

            //AsNoTracking
            var data = db.Course.AsNoTracking();

            foreach (var item in data)
            {
                Console.WriteLine(item.Title);
            }

            #endregion

            #region DbEntityEntry<T>

            var c = new Course()
            {
                Title = "Test",
                Credits = 5,
                DepartmentID = 5
            };

            db.Course.Add(c);

            Console.WriteLine(c.Title + "\t" + db.Entry(c).State);

            db.SaveChanges();

            Console.WriteLine(c.Title + "\t" + db.Entry(c).State);

            c.Credits += 1;

            Console.WriteLine(c.Title + "\t" + db.Entry(c).State);

            db.Course.Remove(c);

            Console.WriteLine(c.Title + "\t" + db.Entry(c).State);

            db.SaveChanges();

            #endregion

            #region DbPropertyValues

            var c2 = db.Course.Find(7);

            c2.Title = "Title 123";

            if (db.Entry(c2).State == System.Data.Entity.EntityState.Modified)
            {
                var ce = db.Entry(c2);
                var v1 = ce.CurrentValues;
                var v2 = ce.OriginalValues;

                Console.WriteLine("\toriginal\tcurrent");

                //DbPropertyValues.SetValues
                v1.SetValues(new
                {
                    Credits = 125
                });

                //DbPropertyValues.GetValue<T>
                foreach (var property in ce.OriginalValues.PropertyNames)
                {
                    var current = v1.GetValue<object>(property);
                    var original = v2.GetValue<object>(property);

                    Console.WriteLine(property);
                    Console.WriteLine(" original:\t" + original);
                    Console.WriteLine(" current:\t" + current);
                }
            }

            #endregion

            #region 深入了解變更追蹤機制

            //參考 ContosoUniversityEntities.partial.cs
            var test = db.Course.Find(7);

            test.Credits += 1;
            db.SaveChanges();

            #endregion
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
