using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SwitchHolder : MonoBehaviour {

	GameObject switchHolder;
	//TODO: Don't keep these as static
	public static List<GameObject> switchGOs = new List<GameObject>();
	public static List<Switch> allSwitches = new List<Switch>();

	public List<GameObject> planetSwitches = new List<GameObject>();
	// Use this for initialization
	void Awake(){
		switchHolder = GameObject.Find("Switch Holder");
		switchGOs = switchHolder.GetComponentsInChildren<GameObject>().ToList();	
		foreach(GameObject go in switchGOs){
			allSwitches.Add(go.GetComponent<Switch>());
		}
	}

	public void PlanetCreated(){
		//Add the action for when a planet's created
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
