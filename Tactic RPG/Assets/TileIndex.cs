using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Comme ca cette classe peut afficher ses varaibles [SerializeField] dans l'inspector
public class TileIndex
{
    [SerializeField]
    int x;
    [SerializeField]
    int z;

    public int X { get => x; set => x = value; }
    public int Z { get => z; set => z = value; }

    public TileIndex(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}
