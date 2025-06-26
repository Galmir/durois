using System;
using Godot;
using Godot.Collections;

public partial class Rules : Node {


    public enum Tiles {
        Floor,
        Door,
        Hallway,
        Entrance,
        Exit,
        Enemy,
        Loot
    }

    public enum CardinalDirections {
        N,
        S,
        W,
        E
    }

    public Dictionary<Tiles, int> TileDict = new Dictionary<Tiles, int>() {
        {Tiles.Floor, 0},
        { Tiles.Door, 1},
        { Tiles.Hallway, 2},
        { Tiles.Entrance, 3},
        { Tiles.Exit, 4},
        { Tiles.Enemy, 5},
        { Tiles.Loot, 6}
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
        Common,
        Uncommon,
        Rare,
        Epic,
        legendary
    }

    public static Dictionary<ItemRarities, Color> RarityColors = new Dictionary<ItemRarities, Color>{
        {ItemRarities.Common,GameConstants.RarityCommonColor },
        {ItemRarities.Uncommon,GameConstants.RarityUncommonColor },
        {ItemRarities.Rare,GameConstants.RarityRareColor },
        {ItemRarities.Epic,GameConstants.RarityEpicColor },
        {ItemRarities.legendary,GameConstants.RarityLegendaryColor }
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
