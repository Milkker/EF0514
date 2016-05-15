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
    }
}
