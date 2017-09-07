using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrader.GW2API.Entities
{
    public class Entity
    {
        [Key]
        public int ID { get; set; }

        public int APIID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime LoadDate { get; set; }
    }
}
