using Godot;
using System.IO;

public partial class SaveLoad : Node {

    private static SaveLoad _instance;
    public static SaveLoad Instance => _instance;
    public override void _Ready() {
        if (_instance != null) {
            _instance.QueueFree(); // The Singletone is already loaded, kill this instance
        }
        _instance = this;
    }

    private string savePath = "user://saveSlot.tres";

    public void SaveGame() {
        SaveData saveSlot = new();
        saveSlot.PlayerInventory = PlayerData.Instance.PlayerInventory;
        saveSlot.CombatData = PlayerData.Instance.CombatData;

        ResourceSaver.Save(saveSlot, savePath);
    }


    public void LoadGame() {
        if (!isSaveSlotPresent()) return;
        SaveData saveSlot = ResourceLoader.Load(savePath).Duplicate(true) as SaveData;
        PlayerData.Instance.PlayerInventory = saveSlot.PlayerInventory;
        PlayerData.Instance.CombatData = saveSlot.CombatData;
    }

    public bool isSaveSlotPresent() {
        return File.Exists(savePath);
    }
}
