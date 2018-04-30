using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameLoader : MonoBehaviour{
    GameStateHandler gameStateHandler;
    void Awake(){
        if(GameStateHandler.instance == null){
            Instantiate(gameStateHandler);
        }
    }
}