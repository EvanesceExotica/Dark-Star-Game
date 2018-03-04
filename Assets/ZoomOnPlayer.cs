using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public static class ZoomOnPlayer {


	 static void Awake(){
	}

    
  	public static void ZoomInOnPlayer(float zoomAmount, float duration, ProCamera2D ourProCamera2D)
    {
        //TODO: ADD SOMETHING LIKE A ZOOMING FLAG SO THAT OTHER ZOOMING DOESN'T INTERRUPT
         ourProCamera2D.RemoveCameraTarget(GameStateHandler.DarkStarGO.transform);
        ourProCamera2D.Reset();
        // ourProCamera2D.Zoom(zoomAmount, duration, EaseType.EaseInOut);
    }

    public static void ZoomOut(float zoomAmount, float duration, ProCamera2D ourProCamera2D)
    {
        ourProCamera2D.Zoom(zoomAmount, duration, EaseType.EaseInOut);
        ourProCamera2D.AddCameraTarget(GameStateHandler.DarkStarGO.transform);
    }
	// Use this for initialization
	
}
