using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFConsole
{
    public partial class ContosoUniversityEntities:DbContext
    {
        public override int SaveChanges()
        {
            var entries = this.ChangeTracker.Entries();

            foreach (var entity in entries)
            {
                if (entity.Entity is Course)
                {
                    //var c = (Course)entity.Entity;
                    //c.ModifiedOn = DateTime.Now;

                    entity.CurrentValues.SetValues(new
                    {
                        ModifiedOn = DateTime.Now
                    });
                }
            }

            return base.SaveChanges();
        }


        protected override System.Data.Entity.Validation.DbEntityValidationResult ValidateEntity(System.Data.Entity.Infrastructure.DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry.Entity is Course)
            {
                if (string.IsNullOrEmpty(entityEntry.CurrentValues.GetValue<string>("Title")))
                {
                    var list = new List<System.Data.Entity.Validation.DbValidationError>();
                    list.Add(new System.Data.Entity.Validation.DbValidationError("Title", "Title 欄位必填"));

                    return new System.Data.Entity.Validation.DbEntityValidationResult(entityEntry, list);
                }
            }

            return base.ValidateEntity(entityEntry, items);
        }
    }
}
