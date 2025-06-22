using System;
using Godot;

public partial class ItemObtainedUi : PanelContainer {
    private InventorySlot obtainedItem;

    public InventorySlot ObtainedItem {
        get { return obtainedItem; }
        set {
            obtainedItem = value;
            ingredientIcon.Texture = obtainedItem.Item.Icon;
            ingredientNameLabel.Text = obtainedItem.Item.Name;
            amountLabel.Text = string.Concat("x", obtainedItem.Amount);

            colorBG.Color = Rules.RarityColors[obtainedItem.Item.ItemRarity];
        }
    }

    public Action ItemObtainedFinished;
    public bool IsEnough { get; private set; }
    [Export] private ColorRect colorBG;
    [Export] private TextureRect ingredientIcon;
    [Export] private Label ingredientNameLabel;
    [Export] private Label amountLabel;
    [Export] private Timer timeToLeave;

    private Tween tween;
    private float xOffset = 300;

    public override void _Ready() {
        timeToLeave.Timeout += OnTimeout;
        tween = GetTree().CreateTween();
        tween.TweenProperty(this, "position:x", -xOffset, 1.0f).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);

    }


    public void OnTimeout() {
        tween = GetTree().CreateTween();
        tween.TweenProperty(this, "position:x", xOffset, 1.0f).AsRelative().FromCurrent().SetTrans(Tween.TransitionType.Expo);
        tween.TweenCallback(Callable.From(() => QueueFree()));
    }
}
