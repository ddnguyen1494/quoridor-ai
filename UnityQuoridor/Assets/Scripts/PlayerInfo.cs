using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInfo : MonoBehaviour {
	
	public GameObject body;
	public Vector3 spawnPoint;

	public int x;
	public int y;
	public int goalX;
	public int goalY;
	public int wallsLeft;
	public int id;
	public bool currentTurn = false;

	public bool checkWin()
	{
		return (x == goalX || y == goalY);
	}


}
