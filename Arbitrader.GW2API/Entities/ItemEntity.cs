using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// A row of data in the Items table. Associated with the result type <see cref="ItemResult"/>.
    /// </summary>
    [Table("Items")]
    public class ItemEntity : Entity
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address of the image used to represent the item in the GW2 UI.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the rarity of the item.
        /// </summary>
        public string Rarity { get; set; }
        
        /// <summary>
        /// Gets or sets the level required to use the item.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Gets or sets the value of the item if it were to be sold to an NPC vendor.
        /// </summary>
        public int? VendorValue { get; set; }

        /// <summary>
        /// Gets or sets the list of flags assigned to the item.
        /// </summary>
        public IList<ItemFlagEntity> Flags { get; set; } = new List<ItemFlagEntity>();

        /// <summary>
        /// Converts from <see cref="ItemResult"/> to its associated entity, <see cref="ItemEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
        public static implicit operator ItemEntity(ItemResult result)
        {
            return new ItemEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate,
                Name = result.name,
                Icon = result.icon,
                Type = result.type,
                Rarity = result.rarity,
                Level = result.level,
                VendorValue = result.vendor_value,
                Flags = result.flags.Select(f => (ItemFlagEntity)f.ToEntity()).ToList()
            };
        }
    }
}