using Godot;

public partial class FSMAI : Node {
    [Export] Label3D debugLabel;
    private StateAI currentState;
    private StateAI previousState;

    public override void _Ready() {
        currentState = GetChild<StateAI>(0);
        previousState = currentState;
        currentState.Enter();
    }

    public override void _PhysicsProcess(double delta) {
    }

    public void ChangeState(string newState) {
        if (currentState.Name == newState) return;
        currentState = FindChild(newState) as StateAI;
        previousState.Exit();
        currentState.Enter();
        previousState = currentState;
        debugLabel.Text = currentState.Name;
    }

}
