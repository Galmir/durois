using Godot;
using Godot.Collections;

[GlobalClass]
public partial class InventoryItem : Resource {
    [Export] public string Name;
    [Export(PropertyHint.MultilineText)] public string LoreDescription;
    [Export] public Texture2D Icon;
    [Export] public int GoldValue;
    [Export] public bool IsStackable;
    [Export] public Array<Rules.ItemTags> FilterTags;
    [Export] public Rules.ItemRarities ItemRarity;

    protected string description;

    protected virtual void UpdateDescription() {
        description = "[p]" + TranslationServer.Translate(LoreDescription) + "[/p]\n";
    }

    public string GetFullDescription() {
        UpdateDescription();
        return description;
    }

}
