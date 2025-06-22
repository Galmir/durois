using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public abstract partial class CombatData : Resource {

    [Export] protected Array<StatBonus> StartingStats;
    [Export] protected int StartingHealth;
    [Export] protected int StartingMana;
    [Export] protected int StartingStamina;
    [Export] protected float HealthRegen;
    [Export] protected float StartingHealthRegen;


    [Export] protected float MaxHealth;
    [Export] protected float MaxMana;
    [Export] protected float MaxStamina;

    // Health
    private float _health;
    public Action<float> HealthChanged;
    public Action TriggerDeath;

    public bool IsDead { get; private set; } = false;
    private float currentHealth {
        get { return _health; }
        set {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            HealthChanged.Invoke(_health);
            if (_health == 0) {
                TriggerDeath?.Invoke();
                IsDead = true;
            }
        }
    }

    // Mana 
    private float _mana;
    public Action<float> ManaChanged;
    private float currentMana {
        get { return _mana; }
        set {
            _mana = Mathf.Clamp(value, 0, MaxHealth);
            ManaChanged?.Invoke(_mana);
        }
    }

    // Stamina
    private float _stamina;
    public Action<float> StaminaChanged;
    private float currentStamina {
        get { return _stamina; }
        set {
            _stamina = Mathf.Clamp(value, 0, MaxHealth);
            StaminaChanged?.Invoke(_stamina);
        }
    }


    public virtual void InitStats() {
        currentHealth = MaxHealth;
        currentMana = MaxMana;
        currentStamina = MaxStamina;
    }


    #region Damage

    /// <summary>
    /// send the full damage array list to apply defenses
    /// </summary>
    /// <param name="attacks">the array of damages in the attack, usually the weapon full damage list</param>
    /// <param name="damageMultiplier"> for effects that use only a percentage of the DPS</param>
    public void ReceiveDamagePayload(Array<DamageData> attacks, float damageMultiplier = 1) {
        foreach (DamageData singleAttack in attacks) {
            ReceiveSingleDamage(singleAttack.RollDamage(), singleAttack.DamageType);
        }
    }

    private void ReceiveSingleDamage(int amount, Rules.DamageTypes type, float damageMultiplier = 1) {
        /*
        Stats defensiveStats = GetDefensiveData(type);
        amount = (int)(amount * damageMultiplier);
        int finalAttackDamage = amount - (int)(amount * defensiveStats.DamageAffectedByPercent) +
                                defensiveStats.FlatDamageAffectedAmount;
        Health - finalAttackDamage;
        */
    }


    #endregion



}
