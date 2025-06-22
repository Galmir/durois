using Godot;

[GlobalClass]
public partial class CraftingRecipe : Resource {
    [Export] public InventoryItem ResultingItem;
    [Export] public int ResultingItemAmount;
    [Export] public Godot.Collections.Array<InventorySlot> RequiredIngredients;

    public string GetRecipeName() {
        return "CRAFTING_RECIPE_NAME_PREFIX" + ResultingItem.Name;
    }
}
