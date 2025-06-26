using Godot;

public partial class LevelManager : Node3D {
    [Export] AudioStream BGM;
    [Export] DungeonGenerator dunGen;
    [Export] Minimap minimap;

    public override void _Ready() {
        dunGen.PlayerPlaced += MinimapCamera;
        dunGen.GenerateDungeon();
        GetTree().CreateTimer(1).Timeout += () => AudioManager.Instance.Play(AudioType.Music, BGM);

    }

    public void MinimapCamera(PlayerController player) {
        minimap.AssignCameraTarget(player);
    }
}
