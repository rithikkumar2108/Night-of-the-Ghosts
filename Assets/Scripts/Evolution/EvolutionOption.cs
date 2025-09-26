using UnityEngine;

[CreateAssetMenu(menuName = "Evolution/Evolution Option")]
public class EvolutionOption : ScriptableObject
{
    [Header("UI")]
    public string optionName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Stat Modifiers (Percentages)")]
    [Tooltip("Percentage change in max health. Ex: 0.2 = +20%, -0.1 = -10%")]
    public float healthMultiplier;

    [Tooltip("Percentage change in movement speed. Ex: 0.3 = +30%, -0.2 = -20%")]
    public float speedMultiplier;

    [Tooltip("Percentage change in fire rate. Ex: 0.5 = +50%, -0.25 = -25%")]
    public float fireRateMultiplier;

    [Tooltip("Percentage change in bullet spread. Ex: -0.3 = -30% tighter spread, +0.3 = +30% wider spread")]
    public float bulletSpreadMultiplier;

    [Tooltip("Percentage change in bullet damage. Ex: 1.0 = +100%, -0.5 = -50%")]
    public float bulletDamageMultiplier;
}
