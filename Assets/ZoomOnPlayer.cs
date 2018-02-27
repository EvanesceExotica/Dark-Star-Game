using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public static class ZoomOnPlayer {

	public static ProCamera2D ourProCamera2D;

	 static void Awake(){
		ourProCamera2D = Camera.main.GetComponent<ProCamera2D>();
	}
  	public static void ZoomInOnPlayer(float zoomAmount, float duration)
    {

        ourProCamera2D.Zoom(zoomAmount, duration, EaseType.EaseInOut);
    }

    public static void ZoomOut(float zoomAmount, float duration)
    {
        ourProCamera2D.Zoom(zoomAmount, duration, EaseType.EaseInOut);
    }
	// Use this for initialization
	
}
