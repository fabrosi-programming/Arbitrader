using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// The base class for all entities that store data retrieved from the GW2 API.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity internal to Arbitrader.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the result associated with the entity.
        /// </summary>
        public int APIID { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the result associated with the entity was pulled from the GW2 API.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime LoadDate { get; set; } = DateTime.Now;
    }
}