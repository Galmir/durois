using Godot;

[GlobalClass]
public partial class IngredientRowUI : Control {
    private InventorySlot ingredient;

    public InventorySlot Ingredient {
        get { return ingredient; }
        set {
            ingredient = value;
            ingredientIcon.Texture = ingredient.Item.Icon;
            ingredientNameLabel.Text = ingredient.Item.Name;

            ownedAmount = PlayerData.Instance.PlayerInventory.GetAmountOfItem(ingredient.Item);

            if (ownedAmount >= ingredient.Amount) {
                IsEnough = true;
                amountLabel.AddThemeColorOverride("font_color", GameConstants.PositiveColor);
            }
            else {
                IsEnough = false;
                amountLabel.AddThemeColorOverride("font_color", GameConstants.NegativeColor);
            }
            amountLabel.Text = string.Concat(ownedAmount, "/", ingredient.Amount);

        }
    }

    public bool IsEnough { get; private set; }

    [Export] private TextureRect ingredientIcon;
    [Export] private Label ingredientNameLabel;
    [Export] public Label amountLabel;

    private int ownedAmount;
}
