using Godot;

public partial class FSM : Node {

    private State currentState;
    private State previousState;

    public override void _Ready() {
        currentState = GetChild<State>(0);

        previousState = currentState;
        currentState.Enter();
    }

    public override void _PhysicsProcess(double delta) {
    }

    public void ChangeState(string newState) {
        if (currentState.Name == newState) return;
        currentState = FindChild(newState) as State;
        previousState.Exit();
        currentState.Enter();
        previousState = currentState;
    }

}
