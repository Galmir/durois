using Godot;

public partial class HUD : CanvasLayer {
    [Export] private ProgressBar playerhealthBar;
    [Export] private ProgressBar playerStaminaBar;


    public override void _Ready() {
        Signals.Instance.HealthChanged += UpdateHealthBar;
        Signals.Instance.StaminaChanged += UpdateStaminaBar;
    }


    private void UpdateHealthBar(PhysicsBody3D who, HealthComponent healthData) {
        if (who.Name == "Player") {
            playerhealthBar.Value = healthData.currenthealth;
            playerhealthBar.MaxValue = healthData.maxHealth;
        }
    }

    private void UpdateStaminaBar(PhysicsBody3D who, StaminaComponent staminaData) {
        if (who.Name == "Player") {
            playerStaminaBar.Value = staminaData.currentStamina;
            playerStaminaBar.MaxValue = staminaData.maxStamina;
        }
    }
}
