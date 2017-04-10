using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API
{
    public partial class Discipline
    {
        public static implicit operator Discipline(string disciplineName)
        {
            var entities = new ArbitraderEntities();
            var disciplines = from discipline in entities.Disciplines
                              where discipline.name == disciplineName
                              select discipline;

            if (disciplines.Count() > 0)
                return disciplines.FirstOrDefault();
            else
            {
                var newDiscipline = new Discipline()
                {
                    name = disciplineName
                };
                entities.Disciplines.Add(newDiscipline);
                entities.SaveChanges();
                return newDiscipline;
            }
        }
    }
}
