﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyGroup : MonoBehaviour {

	public List<SpaceMonster> enemyTypes; 

	public BlueDwarf blueDwarfPrefab;
	public EventHorizon eventHorizonPrefab;

	public enum EnemyTypes{
		BlueDwarf,

		EventHorizon,
		Bumper,
		Comet

	}

	public Dictionary<EnemyTypes, SpaceMonster> correspondingSpaceMonster_ = new Dictionary<EnemyTypes, SpaceMonster>();
	public Dictionary<int, SpaceMonster> correspondingSpaceMonster = new Dictionary<int, SpaceMonster>();
	public Dictionary<SpaceMonster, int> correspondingSpaceMonsterIndex = new Dictionary<SpaceMonster, int>();
	public List<SpaceMonster> IDIndexCorrespondingToSpaceMonster = new List<SpaceMonster>();

	void Awake(){
		correspondingSpaceMonsterIndex.Add(blueDwarfPrefab, 0);
		correspondingSpaceMonsterIndex.Add(eventHorizonPrefab, 1);
		correspondingSpaceMonster.Add(0, blueDwarfPrefab);
		IDIndexCorrespondingToSpaceMonster.Insert(0, blueDwarfPrefab);
		correspondingSpaceMonster.Add(1, eventHorizonPrefab);
		IDIndexCorrespondingToSpaceMonster.Insert(1, eventHorizonPrefab);
	}

	
	public class DictionaryOfSpaceMonsterAndGameObject : SerializableDictionary<SpaceMonster, GameObject> { }

	public DictionaryOfSpaceMonsterAndGameObject thisLevelsEnemyGroup ;
	public List<KeyValuePair<SpaceMonster, GameObject>> levelEnemyGroup;
}
