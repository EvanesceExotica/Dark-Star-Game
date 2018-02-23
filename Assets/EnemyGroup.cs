using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyGroup")]
public class EnemyGroup : ScriptableObject {

	public List<SpaceMonster> enemyTypes; 

	public enum EnemyTypes{
		BlueDwarf,
		EventHorizon,
		Comet
	}

	public Dictionary<EnemyTypes, int> correspondingEnemyThreatLevel = new Dictionary<EnemyTypes, int>();
	public Dictionary<EnemyTypes, SpaceMonster> correspondingSpaceMonster = new Dictionary<EnemyTypes, SpaceMonster>();

	void PopulateTypes(){
	} 

	
	public class DictionaryOfSpaceMonsterAndGameObject : SerializableDictionary<SpaceMonster, GameObject> { }

	public DictionaryOfSpaceMonsterAndGameObject thisLevelsEnemyGroup ;
	public List<KeyValuePair<SpaceMonster, GameObject>> levelEnemyGroup;
}
