﻿using System.Collections;
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

	public bool CheckWin()
	{
		return (x == goalX || y == goalY);
	}

    public int GetDistanceToGoal()  
    {
        return goalX - x;//2 Players for now
    }
}
