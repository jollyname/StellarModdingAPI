using System.Collections.Generic;

namespace StellarModdingAPI.Parts
{
    public class RecipeIngredient
    {
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
    }

    public class RecipeDefinition
    {
        public List<RecipeIngredient> Ingredients { get; } = [];
    }
}