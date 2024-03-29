﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// A row of data in the GuildIngredients table. Associated with the result type <see cref="GuildIngredientResult"/>.
    /// </summary>
    [Table("GuildIngredients")]
    public class GuildIngredientEntity : Entity
    {
        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the guild hall upgrade that requires the ingredient.
        /// </summary>
        public int UpgradeID { get; set; }

        /// <summary>
        /// Gets or sets the number of the ingredient required for the guild hall upgrade.
        /// </summary>
        public int Count { get; set; }
    }
}