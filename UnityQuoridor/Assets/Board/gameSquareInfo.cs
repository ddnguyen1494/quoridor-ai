using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameSquareInfo : MonoBehaviour {

    public gameSquareInfo(gameSquareInfo other)
    {
        hasBotWall = other.hasBotWall;
        hasRightWall = other.hasRightWall;
        isOpen = other.isOpen;
        x = other.x;
        y = other.y;
    }

	public bool hasBotWall = false;
	public bool hasRightWall = false;
	public bool isOpen = true;
	public Vector3 location;

	public int x;
	public int y;
		


}
