using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DontDestroyOnLoad_ : MonoBehaviour{
   public void Awake(){
       DontDestroyOnLoad(gameObject);
   } 
}