using Godot;

public partial class LevelManager : Node3D {
    [Export] AudioStream BGM;
    [Export] DungeonGenerator dunGen;
    [Export] Minimap minimap;
    [Export] CanvasLayer inventoryCanvas;
    [Export] CanvasLayer charSheetCanvas;

    public override void _Ready() {
        dunGen.PlayerPlaced += MinimapCamera;
        dunGen.GenerateDungeon();
        GetTree().CreateTimer(1).Timeout += () => AudioManager.Instance.Play(AudioType.Music, BGM);

    }

    public void MinimapCamera(PlayerController player) {
        minimap.AssignCameraTarget(player);
    }

    public override void _Input(InputEvent @event) {
        if (@event.IsActionReleased("toggle_inventory")) {
            if (!inventoryCanvas.Visible) {
                inventoryCanvas.Visible = true;
                charSheetCanvas.Visible = false;
            }
            else inventoryCanvas.Visible = false;
        }
        if (@event.IsActionReleased("toggle_character_sheet")) {
            if (!charSheetCanvas.Visible) {
                charSheetCanvas.Visible = true;
                inventoryCanvas.Visible = false;
            }
            else charSheetCanvas.Visible = false;
        }
    }
}
