using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightTile : Tile
{
    bool topTile = false;
    BaseTile baseTile;
    public bool TopTile { get => topTile; set => topTile = value; }
    public BaseTile BaseTile { get => baseTile; set => baseTile = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
