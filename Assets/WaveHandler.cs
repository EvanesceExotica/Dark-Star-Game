using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour {

    int maxNumberOfWaves;



    public float waveChance;

    bool deployingWaves;

    float timeBetweenWaves;

    float lastWaveStartTime;

    float minimumWaveStartTime;
    float maximumWaveStartTime;

    float nextWaveTime;

    bool FindChance(float chance)
    {
        bool isTriggered = false;
        if (UnityEngine.Random.value <= chance)
        {
            isTriggered = true;
        }
      
        return isTriggered;
    }

    public float HandleWave()
    {
      /*  if (FindChance(/rainChance))
        {*/
            //rainToday = true;
            nextWaveTime = UnityEngine.Random.Range(minimumWaveStartTime, maximumWaveStartTime);
        return nextWaveTime;
       // }
    }

    public IEnumerator DoleOutWaves()
    {
        lastWaveStartTime = Time.time;

        yield return null;
    }

	
	// Update is called once per frame
	void Update () {
     //   while(Time.time + lastWaveStartTime < )
		
	}
}
