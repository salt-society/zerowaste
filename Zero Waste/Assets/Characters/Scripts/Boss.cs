using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Character/Boss")]
public class Boss : Enemy
{
    [Header("Boss Information")]
    public Color[] phaseColors;

    // Definition for how many phases we want to have
    public float[] phaseNumbers;

    // Definition for how many effects to add per phase
    public int[] effectNumber;

    // Effects to add when switching to next phase
    public Effect[] phaseEffects;

    // Storage for HP thresholds to reduce computation time
    [HideInInspector] public int[] thresholds;

    // Boolean to check if phases has been cleared
    [HideInInspector] public bool[] hasClearedPhase;

    // Override initialize enemy to initialize boolean array and calculate PL to switch phase to
    public override void OnInitialize()
    {
        base.OnInitialize();

        // Initialize storage arrays to match length of phases
        hasClearedPhase = new bool[phaseNumbers.Length];
        thresholds = new int[phaseNumbers.Length];

        // Initialize all to false at the start of battle
        for (int CTR = 0; CTR < hasClearedPhase.Length; CTR++)
            hasClearedPhase[CTR] = false;

        // Calculate thresholds for when to switch to next phase
        for (int CTR = 0; CTR < phaseNumbers.Length; CTR++)
            thresholds[CTR] = (int)((float)basePollutionLevel * phaseNumbers[CTR]);

        Debug.Log("Init Boss.");
    }

    // Override isAttacked to accomodate phase switching
    public override void IsAttacked(int statModifier, Player attacker)
    {
        base.IsAttacked(statModifier, attacker);

        for (int CTR = 0; CTR < thresholds.Length; CTR++)
        {
            if (!hasClearedPhase[CTR])
            {
                if (currentPollutionLevel <= thresholds[CTR])
                {
                    PhaseSwitch(CTR);
                    hasClearedPhase[CTR] = true;
                }
            }
        }
    }

    // Function for switching to another phase
    private void PhaseSwitch(int phaseNumber)
    {
        int effectStart = 0;
        int effectEnd = 0;

        if (phaseNumber > 0)
        {
            for (int CTR = phaseNumber; CTR > 0; CTR--)
                effectStart += effectNumber[CTR];

            for (int CTR = phaseNumber; CTR >= 0; CTR--)
                effectEnd += effectNumber[CTR];
        }
        else
        {
            effectEnd = effectNumber[phaseNumber];
        }

        while (effectStart < effectEnd)
        {
            this.IsBuffed(Instantiate(phaseEffects[effectStart]));
            effectStart++;
        }
    }
}
