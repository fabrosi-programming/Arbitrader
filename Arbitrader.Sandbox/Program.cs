using Arbitrader.GW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var json_item = @"{
                'name':'Recipe: Satchel of Rejuvenating Masquerade Armor(Rare)',
                'description':'Double - click to learn the Tailor recipe for Satchel of Rejuvenating Masquerade Armor(Rare)',
                'type':'Consumable',
                'level':0,
                'rarity':'Rare',
                'vendor_value':0,
                'game_types':['Wvw','Dungeon','Pve'],
                'flags':['NoSalvage'],
                'restrictions':[],
                'id':10000,
                'chat_link':'[&AgEQJwAA]',
                'icon':'https://render.guildwars2.com/file/0D9835CC3F70CC70EF0F2655C764C728580E6CEE/849411.png',
                'details':{
                    'type':'Unlock',
                    'unlock_type':'CraftingRecipe',
                    'recipe_id':2756
                    }
                }";

            var item = Item.FromJson(json_item);

            var json_listing = @"{
                'id':10000,
                'buys':[
                    {
                        'listings':1,
                        'unit_price':10000,
                        'quantity':1
                    },
                    {
                        'listings':1,
                        'unit_price':7501,
                        'quantity':1
                    },
                    {
                        'listings':1,
                        'unit_price':6100,
                        'quantity':1
                    },
                    {
                        'listings':1,
                        'unit_price':3000,
                        'quantity':1
                    },
                    {
                        'listings':1,
                        'unit_price':5,
                        'quantity':1
                    }
                ],
                'sells':[]
                }";

            var listingCollection = ListingCollection.FromJson(json_listing);
        }
    }
}