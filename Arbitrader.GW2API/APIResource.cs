using System.Collections.Generic;

namespace Arbitrader.GW2API
{
    public enum APIResource
    {
        None = 0,
        Items,
        Recipes,
        CommerceListings
    }

    public static class APIResourceExtensions
    {
        private static Dictionary<APIResource, string> pathMapping = new Dictionary<APIResource, string>()
        {
            { APIResource.Items, "items" },
            { APIResource.Recipes, "recipes" },
            { APIResource.CommerceListings, "commerce/listings" }
        };

        public static string GetPath(this APIResource resource)
        {
            return pathMapping[resource];
        }
    }
}