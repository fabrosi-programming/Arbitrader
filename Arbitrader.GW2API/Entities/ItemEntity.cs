using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("Items")]
    public class ItemEntity : Entity
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public int? Level { get; set; }
        public int? VendorValue { get; set; }
        public IList<ItemFlagEntity> Flags { get; set; } = new List<ItemFlagEntity>();

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