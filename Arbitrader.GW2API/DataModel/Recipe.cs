using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API
{
    public partial class Recipe : ICanHazID
    {
        public List<Discipline> Disciplines
        {
            get
            {
                var entities = new ArbitraderEntities();
                return (from rd in entities.RecipeDisciplines
                        where rd.recipePK == this.pk
                        select rd.Discipline).ToList();
            }
            set
            {
                var entities = new ArbitraderEntities();

                var oldRecipeDisciplines = from rd in entities.RecipeDisciplines
                                           where rd.recipePK == this.pk
                                           select rd;
                entities.RecipeDisciplines.RemoveRange(oldRecipeDisciplines);
                entities.SaveChanges();

                var newRecipeDisciplines = from discipline in value
                                           select new RecipeDiscipline()
                                           {
                                               Discipline = discipline,
                                               disciplinePK = discipline.pk,
                                               Recipe = this,
                                               recipePK = this.pk
                                           };
                entities.RecipeDisciplines.AddRange(newRecipeDisciplines);
                entities.SaveChanges();
            }
        }
    }
}
