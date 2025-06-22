using Godot;

public partial class CraftingUI : Control {

    [Export] private PackedScene ingredientRow;
    // UI component to connect
    private VBoxContainer recipeListContainer;
    private Button craftItemButton;
    private Label recipeNameLabel;
    private RichTextLabel recipeDescriptionLabel;
    private TextureRect recipeIcon;

    private VBoxContainer ingredientList;

    // external UI components to load
    private PackedScene recipeButtonScene = GD.Load<PackedScene>("uid://cenh3pgkafiip");
    private CraftingRecipeUIButton recipeButton;

    private CraftingRecipe selectedRecipe;


    public void ClearRecipeList() {
        foreach (CraftingRecipeUIButton recipe in recipeListContainer.GetChildren()) {
            recipe.QueueFree();
        }
    }


    public void ClearRecipeDescription() {
        recipeNameLabel.Text = "";
        recipeDescriptionLabel.Text = "";
        recipeIcon.Texture = null;
    }


    public override void _Ready() {
        recipeButton = recipeButtonScene.Instantiate<CraftingRecipeUIButton>();
        recipeListContainer = GetNode<VBoxContainer>("%RecipeListVbox");
        recipeNameLabel = GetNode<Label>("%RecipeNameLabel");
        recipeDescriptionLabel = GetNode<RichTextLabel>("%RecipeDescription");
        recipeIcon = GetNode<TextureRect>("%RecipeIconImage");
        craftItemButton = GetNode<Button>("%CraftItemButton");
        ingredientList = GetNode<VBoxContainer>("%IngredientList");
        craftItemButton.Disabled = true;
        craftItemButton.Pressed += FinalizeCrafting;

        ClearRecipeList();
        ClearRecipeDescription();
        ClearIngredientList();

        foreach (CraftingRecipe newRecipe in PlayerData.Instance.PlayerInventory.KnownRecipes) {
            CraftingRecipeUIButton newRecipeButton = new CraftingRecipeUIButton();
            recipeListContainer.AddChild(newRecipeButton);
            newRecipeButton.RecipeToShow = newRecipe;
            newRecipeButton.OnRecipeSelected += UpdateRecipeDescription;
        }
    }


    public void UpdateRecipeDescription(CraftingRecipe recipe) {
        selectedRecipe = recipe;
        recipeNameLabel.Text = recipe.ResultingItem.Name;
        recipeDescriptionLabel.Text = recipe.ResultingItem.GetFullDescription();
        recipeIcon.Texture = recipe.ResultingItem.Icon;

        ClearIngredientList();
        craftItemButton.Disabled = false;

        foreach (InventorySlot ingredient in recipe.RequiredIngredients) {

            IngredientRowUI newIngredient = ingredientRow.Instantiate<IngredientRowUI>();
            ingredientList.AddChild(newIngredient);
            newIngredient.Ingredient = ingredient;
            if (!newIngredient.IsEnough) craftItemButton.Disabled = true;
        }
    }


    public void ClearIngredientList() {
        foreach (Node ingredient in ingredientList.GetChildren()) {
            ingredient.QueueFree();
        }



    }


    public void FinalizeCrafting() {
        PlayerData.Instance.PlayerInventory.CraftItemFromRecipe(selectedRecipe);
        UpdateRecipeDescription(selectedRecipe);
    }
}
