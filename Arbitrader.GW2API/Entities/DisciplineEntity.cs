using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// A row of data in the Disciplines table. Associated with the result type <see cref="DisciplineResult"/>.
    /// </summary>
    [Table("Disciplines")]
    public class DisciplineEntity : Entity
    {
        /// <summary>
        /// Gets or sets the name of the discipline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Converts from <see cref="DisciplineResult"/> to its associated entity, <see cref="DisciplineEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
        public static implicit operator DisciplineEntity(DisciplineResult result)
        {
            return (DisciplineEntity)result.ToEntity();
        }
    }
}