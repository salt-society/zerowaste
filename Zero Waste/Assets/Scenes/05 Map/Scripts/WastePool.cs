using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Waste Pool", menuName = "Map/Waste Pool")]
public class WastePool : ScriptableObject
{
    [Header("List of Wastes")]
    public Enemy[] wastePool;

    [Header("Waste Spawn Rates")]
    public int[] spawnRate;

    [Header("Node Level Details")]
    public int baseLevel;
    public int maxLevel;

    // Function for getting the array of wastes to be spawned
    public Enemy[] SelectWatesFromPool()
    {
        // First find how many wastes will be spawned
        int numberOfWastes = NumberOfWastes();

        Enemy[] wasteTeam = new Enemy[numberOfWastes];

        for(int CTR = 0; CTR < numberOfWastes; CTR++)
        {
            int max = 0;

            for (int i = 0; i < spawnRate.Length; i++)
                max += spawnRate[i];

            for (int check = 0; check < spawnRate.Length; check++)
            {
                // Randomize a number from 1 to max + 1 (exclusive max number)
                int rand = Random.Range(1, max + 1);

                // If randomized number fits into spawn rate, add waste to wasteTeam
                if (rand <= spawnRate[check])
                {
                    wasteTeam[CTR] = wastePool[check];

                    // Test Stuff
                    wasteTeam[CTR].baseLevel = baseLevel;
                    wasteTeam[CTR].maxLevel = maxLevel;
                }
                    
                // If not, reduce the max by subtracting the spawnrate of current enemy. By doing this, there is always a guarantee
                // that one monster will be chosen from the pool
                else
                    max -= spawnRate[check];
            }
        }

        return wasteTeam;
    }

    // Functions for getting the number of wastes to be spawned
    private int NumberOfWastes()
    {
        // Roll the dice to determine the number of wastes to be spawned
        int randomizedNumber = Random.Range(1, 11);

        // 60% (5,6,7,8,9,10) to spawn 3 Wastes
        if (randomizedNumber > 4)
            return 3;

        // 30% (4,3,2) to spawn 2 Wastes
        else if (randomizedNumber > 1)
            return 2;

        // 10% (1) to spawn 1 Waste
        else
            return 1;
    }
}
