using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsBuilTower { set; get; }
    private void Awake()
    {
        IsBuilTower = false;
    }
}
