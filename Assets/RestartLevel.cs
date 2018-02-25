using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{

    void LevelRestart()
    {
        StartCoroutine(LevelRestartCoroutine());
    }

    public IEnumerator LevelRestartCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);



    }

	void Awake(){
		GameStateHandler.levelFailed += LevelRestart;
	}
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			LevelRestart();
		}

    }
}
