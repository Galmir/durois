using System;
using Godot;

[GlobalClass]
public partial class CraftingRecipeUIButton : Button {

    private CraftingRecipe recipeToShow;

    [Export]
    public CraftingRecipe RecipeToShow {
        get { return recipeToShow; }
        set {
            recipeToShow = value;
            Icon = recipeToShow.ResultingItem.Icon;
            Text = recipeToShow.ResultingItem.Name;
            if (recipeToShow.ResultingItem.IsStackable)
                Text += " " + recipeToShow.ResultingItemAmount.ToString();
        }
    }


    public Action<CraftingRecipe> OnRecipeSelected;


    public override void _Pressed() {
        OnRecipeSelected?.Invoke(RecipeToShow);
    }
}
