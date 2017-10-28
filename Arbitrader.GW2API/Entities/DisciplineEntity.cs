﻿using System.ComponentModel.DataAnnotations.Schema;
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
    }
}