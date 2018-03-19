using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "custom/Wave", order = 1)]
public class Wave : ScriptableObject
{
    public enum EnemyTypes
    {
        BlueDwarf,

        EventHorizon,
        Bumper,
        Comet

    }


    //i.e., in index 0, which corresponds to blue dwarf, have 5, in index 1, which corresponds to Event Horizon, have 2

    public List<int> ratiosCorrespondedToType = new List<int>();

    public int highestEnemyIDToAppearInWave;
    public List<SpaceMonster> ourSpaceMonsterTypes = new List<SpaceMonster>();
    public Dictionary<int, SpaceMonster> ourSpaceMonsterTypes_ = new Dictionary<int, SpaceMonster>();
	public int numberPerWave;

    public List<int> PopulateWaves()
    {

        highestEnemyIDToAppearInWave = ratiosCorrespondedToType.Count - 1;
          numberPerWave = AddUpTypesPerWave();
        List<int> currentWave = new List<int>();
        for (int i = 0; i <= ratiosCorrespondedToType.Count - 1; i++)
        {//i.e, we have 5 in index 0 and 3 in index 1 (5 blue dwarves, 3 event horizons)
            for (int k = 0; k < ratiosCorrespondedToType[i]; k++)
            {
                //e.g., for 5 ticks in the index blue dwarves, add i (which is at first 0, corresponding to blue dwarf)  to the current wave
                //next step around in index 1 (event horizon, for 3 ticks, we're going to add 1, which is the index corresponding to event horizon )
                currentWave.Add(i);

            }
        }

        return currentWave;


    }

    int AddUpTypesPerWave()
    {
        //this should add up the total number of enemies per wave. For example, if we have 5 Blue Dwarves and 3 Event Horizons, sum should add up to 8
        int sum = 0;
        foreach (int enemyRatioInteger in ratiosCorrespondedToType)
        {
            sum += enemyRatioInteger;
        }
        return sum;
    }
    // Use this for initialization

}
