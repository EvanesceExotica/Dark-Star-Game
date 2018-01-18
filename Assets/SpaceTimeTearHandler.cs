using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTimeTearHandler : MonoBehaviour {

    public List<GameObject> randomTearTextures;
    public List<GameObject> activeTears;
    public GameObject baseTearPrefab;

    float duration = 3f;

    void CreateTears(List<Vector2> tearLocations)
    {
        
    }


    IEnumerator FormTears(List<Vector2> tearLocations)
    {
        int i = 0;
        float startTime = Time.time;
        while(i < tearLocations.Count)
        {
            Instantiate(baseTearPrefab, tearLocations[i], Quaternion.identity, this.gameObject.transform);
            i++;
            yield return new WaitForSeconds(1.5f);
        }
        //foreach (Vector2 location in tearLocations)
        //{
        //    Instantiate(baseTearPrefab, location, Quaternion.identity, this.gameObject.transform);
        //}
    }

    private void OnEnable()
    {
        SpacetimeSlingshot.RipSpaceTime += this.CreateTears;
    }

    private void OnDisable()
    {
        SpacetimeSlingshot.RipSpaceTime -= this.CreateTears;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
