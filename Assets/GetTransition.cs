using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetTransition {

	public static Vector2 GetTransitionDirection(Vector2 ourPosition, Vector2 targetPosition){

        Vector2 trans = targetPosition - ourPosition;
        trans.Normalize();
		return trans;
	}

	public static Vector2 GetTransitionDirectionWithMouse(Vector2 ourPosition, Vector2 mousePosition){

        Vector2 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector2(mousePosition.x, mousePosition.y));
        Vector2 trans = mousePositionWorld - ourPosition;
        trans.Normalize();
		return trans;
	}

}
