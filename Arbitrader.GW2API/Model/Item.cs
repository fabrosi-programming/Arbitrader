using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Model
{
    public class Item
    {
        private string _icon;
        private string _type;
        private string _rarity;
        private int? _level;
        private int? _vendor_value;
        private Collection<Flag> _flags = new Collection<Flag>();

        public string Name { get; set; }
        public int ID { get; set; }
        public Collection<Recipe> DependentRecipes { get; set; } = new Collection<Recipe>();
        public Collection<Recipe> GeneratingRecipes { get; set; } = new Collection<Recipe>();

        public Item(ItemEntity itemResult) //TODO: rename to itemEntity
        {
            this._icon = itemResult.Icon;
            this._type = itemResult.Type;
            this._rarity = itemResult.Rarity;
            this._level = itemResult.Level;
            this._vendor_value = itemResult.VendorValue;

            this.Name = itemResult.Name;
            this.ID = itemResult.APIID;

            foreach (var flagResult in itemResult.Flags)
                this._flags.Add((Flag)Enum.Parse(typeof(Flag), flagResult.Name));
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
