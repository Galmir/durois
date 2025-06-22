using Godot;

public partial class LevelManager : Node3D {
    [Export] AudioStream BGM;
    [Export] DungeonGenerator dunGen;

    public override void _Ready() {
        dunGen.GenerateDungeon();
        GetTree().CreateTimer(1).Timeout += () => AudioManager.Instance.Play(AudioType.Music, BGM);
    }
}
