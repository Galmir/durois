using System;
using Godot;
using Godot.Collections;

public partial class Rules : Node {


    public enum Tiles {
        Floor,
        Hallway,
        Door,
        Border,
        Wall,
        Entrance,
        Exit,
        Enemy,
        Loot
    }

    public Dictionary<Tiles, int> TileDict = new Dictionary<Tiles, int>() {
        {Tiles.Floor, 0},
        { Tiles.Hallway, 1},
        { Tiles.Door, 2},
        { Tiles.Border, 3},
        { Tiles.Wall, 4},
        { Tiles.Entrance, 5},
        { Tiles.Exit, 6},
        { Tiles.Enemy, 7},
        { Tiles.Loot, 8}
    };

    public enum DamageTypes {
        Physical,
        Fire,
        Ice,
        Thunder,
        Toxic,
        Light
    }

    public enum AilmentTypes {
        Bleed,
        Burn,
        Freeze,
        Shock,
        Poison,
        Blind,
        Slow
    }

    public enum ItemTags {
        ALL,
        Consumable,
        Material,
        Weapon,
        Armor,
        Quest
    }

    public enum ItemRarities {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY
    }

    public static Dictionary<ItemRarities, Color> RarityColors = new Dictionary<ItemRarities, Color>{
        {ItemRarities.COMMON,GameConstants.RarityCommonColor },
        {ItemRarities.UNCOMMON,GameConstants.RarityUncommonColor },
        {ItemRarities.RARE,GameConstants.RarityRareColor },
        {ItemRarities.EPIC,GameConstants.RarityEpicColor },
        {ItemRarities.LEGENDARY,GameConstants.RarityLegendaryColor }
    };

    public enum Attributes {
        Health,
        HealthRegen,
        Mana,
        ManaRegen,
        Stamina,
        StaminaRegen,
        CriticalChance,
        CriticalDamage
    }

    private static Rules _instance;
    public static Rules Instance => _instance;

    public override void _EnterTree() {
        if (_instance != null) Instance.QueueFree();
        _instance = this;
    }
}
