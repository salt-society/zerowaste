using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMonitor : MonoBehaviour
{
    private Player scavenger;
    private Enemy mutant;

    private StatusManager statusManager;

    private bool characterInitialized = false;
    private double maxHealth;
    private double currentHealth;
    private double maxAnt;
    private double currentAnt;
    private string characterType;

    private int position;

    public void SetCharacter(Player scavenger)
    {
        this.scavenger = scavenger;

        maxHealth = scavenger.baseHP + (int)((scavenger.currentLevel - 1) * scavenger.hpModifier);
        currentHealth = scavenger.currentHP;
        maxAnt = scavenger.baseAnt;
        currentAnt = scavenger.currentAnt;

        characterType = "Scavenger";
    }

    public void SetCharacter(Enemy mutant)
    {
        this.mutant = mutant;

        maxHealth = mutant.maxPollutionLevel;
        currentHealth = mutant.currentPollutionLevel;
        characterType = "Mutant";

    }

    public void SetPosition(int position)
    {
        this.position = position;
    }

    public int GetPosition()
    {
        return position;
    }

    public void UpdateHealth(double health)
    {
        currentHealth = health;
    }

    public double GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetStatusManager(StatusManager statusManager)
    {
        this.statusManager = statusManager;
    }

    public Character GetCharacterData(int characterIndex)
    {
        return (characterIndex > 0) ? scavenger as Character : mutant as Character;
    }

    public int GetCharacterInstance(int characterIndex)
    {
        return (characterIndex > 0) ? scavenger.GetInstanceID() : mutant.GetInstanceID();
    }

    public bool CheckCharacterType(string targetType)
    {
        return (this.characterType.Equals(targetType)) ? true : false;
    }
}
