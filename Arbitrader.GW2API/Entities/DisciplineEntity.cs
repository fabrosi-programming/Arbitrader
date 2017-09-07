using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("Disciplines")]
    public class DisciplineEntity : Entity
    {
        public string Name { get; set; }

        public static implicit operator DisciplineEntity(DisciplineResult result)
        {
            return (DisciplineEntity)result.ToEntity();
        }
    }
}
