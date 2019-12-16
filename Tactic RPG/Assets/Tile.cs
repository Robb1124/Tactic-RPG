using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool usedByCharacter = false;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool attackSelectable = false;
    bool forAttack = false;
    public TileIndex index;
    public List<Tile> adjacencyList = new List<Tile>();
    Actor characterOnTile;
    TileManager tileManager;
    Tile[,] tiles;
    float characterJumpHeight;
    //for BFS
    public bool visited = false; //avec BFS on visite 1 fois seulement une tile
    public Tile parent = null; //pour keep track du chemin
    public int distance = 0; //distance from starting tile

    public Actor CharacterOnTile { get => characterOnTile; set => characterOnTile = value; }

    public void SetTileIndex(TileIndex index)
    {
        this.index = index;        
    }

    // Start is called before the first frame update
    void Awake()
    {
        tileManager = GetComponentInParent<TileManager>();
    }

    public void CheckIfCharacterOnTile()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1)) //Raycasts vers le haut pour savoir si ya un collider
        {
            if (hit.collider.GetComponent<Actor>())
            {
                usedByCharacter = true; //La tile est occupied
                characterOnTile = hit.collider.GetComponent<Actor>();
                characterOnTile.CharacterTileIndex = index; //On enregistre la position du character dans le grid.
            }
            else
            {
                usedByCharacter = false;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.cyan;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (attackSelectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
    public void Reset() //On reset les tiles pour la prochaine recherche
    {
        walkable = true;
        current = false;
        //target = false; //on le reset manually pour afficher plus longtemps le selected tile.
        selectable = false;
        attackSelectable = false;
        adjacencyList.Clear();    
        visited = false;
        parent = null;
        distance = 0; 
    }

    public void FindNeighbors(float jumpHeight, bool forAttack = false) //Les tiles vont aller sonder les tiles voisines
    {
        this.forAttack = forAttack;
        tiles = tileManager.Tiles;
        Reset();
        CheckTile(Direction.Left, jumpHeight); //Left
        CheckTile(Direction.Up, jumpHeight); //Up
        CheckTile(Direction.Right, jumpHeight); //Right
        CheckTile(Direction.Down, jumpHeight);//Down
        
        
    }

    public void CheckTile(Direction direction, float jumpHeight) //On regarde si ya une tile dans la direction demandee, si y'en a, on check si elle est walkable ou used par un character selon les besoins (attack ou mouvement)
    {
        this.characterJumpHeight = jumpHeight;
        Tile tile;
        switch (direction)
        {
            case Direction.Left:
                if (index.X - 1 >= 0)
                {
                    tile = tiles[index.X - 1, index.Z] ? tiles[index.X - 1, index.Z] : null;
                }
                else
                    return;
                break;
            case Direction.Up:
                if (index.Z - 1 > 0)
                {
                    tile = tiles[index.X, index.Z - 1] ? tiles[index.X, index.Z - 1] : null;
                }
                else
                    return;
                break;
            case Direction.Right:
                if (index.X + 1 < tileManager.Width)
                {
                    tile = tiles[index.X + 1, index.Z] ? tiles[index.X + 1, index.Z] : null;
                }
                else
                    return;
                break;
            case Direction.Down:
                if (index.Z + 1 < tileManager.Height)
                {
                    tile = tiles[index.X, index.Z + 1] ? tiles[index.X, index.Z + 1] : null;
                }
                else
                    return;
                break;
            default:
                tile = null;
                break;
        }
        if (IsTileAvailable(tile))
        {
            adjacencyList.Add(tile);    
        }
    }

    bool IsTileAvailable(Tile tile)
    {
        if (forAttack)
        {
            return (tile); //pt qu'on veut pas que les case not walkable ou avec obstacles soit selectable, a voir.
        }
        return (tile && tile.walkable && !tile.usedByCharacter);        
    }
}
