using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPeg : MonoBehaviour
{
    public WallPeg(WallPeg other)
    {
        x = other.x;
        y = other.y;
        isOpen = other.isOpen;
    }

	public int x;
	public int y;
	public bool isOpen = true;
}

