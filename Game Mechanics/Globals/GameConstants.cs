using Godot;

public static partial class GameConstants {
    // UI Colors
    public static Color PositiveColor => new Color("#c2d64f");
    public static Color NegativeColor => new Color("#c33c40");
    public static Color RarityCommonColor => new Color("#696f80");
    public static Color RarityUncommonColor => new Color("#3d80a3");
    public static Color RarityRareColor => new Color("#eff37c");
    public static Color RarityEpicColor => new Color("#c57835");
    public static Color RarityLegendaryColor => new Color("#5c2b34");

    public static Godot.Collections.Array<string> loadingHints = [
        "Wolfs hunt in packs",
        "People die when they are killed",
        "Be careful of the mind goblin",
        "No there's no harem",
        "Dungeon Meshi... Oh, Dungeon Meshi!"
        ];

    public enum EnemyIDs {
        TRAINING_DUMMY
    }
}
