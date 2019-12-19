using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : Tile
{
    [SerializeField] int tilesOnTop;
    HeightTile topTile;
    public bool walkable = true;
    public bool usedByCharacter = false;
    public bool current = false;

    bool forAttack = false;
    public TileIndex index;
    public List<BaseTile> adjacencyList = new List<BaseTile>();
    Actor characterOnTile;
    TileManager tileManager;
    BaseTile[,] tiles;

    //for BFS
    public bool visited = false; //avec BFS on visite 1 fois seulement une tile
    public BaseTile parent = null; //pour keep track du chemin
    public int distance = 0; //distance from starting tile
    float highestTileReachableFromThisTile;
    float lowestTileReachableFromThisTile;

    public Actor CharacterOnTile { get => characterOnTile; set => characterOnTile = value; }
    public int TilesOnTop { get => tilesOnTop; set => tilesOnTop = value; }
    public HeightTile TopTile { get => topTile; set => topTile = value; }

    public void SetTileIndex(TileIndex index)
    {
        this.index = index;
    }

    // Start is called before the first frame update
    void Awake()
    {
        tileManager = GetComponentInParent<TileManager>();
    }

    public void CalculateTilesHeight()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.up, 20, 1 << 8);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<HeightTile>())
            {
                TilesOnTop++;
            }
        }
        if (TilesOnTop > 0)
        {

            Collider[] tile = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y + (0.5f * TilesOnTop), transform.position.z), 0.1f, 1 << 8);
            TopTile = tile[0].GetComponent<HeightTile>();
            TopTile.TopTile = true;
            TopTile.BaseTile = this;
        }
    }

    public void CheckIfCharacterOnTile()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 20, 1 << 9)) //Raycasts vers le haut pour savoir si ya un collider
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
        Color color;
        if (current)
        {
            color = Color.cyan;
        }
        else if (target)
        {
            color = Color.green;
        }
        else if (selectable)
        {
            if (TopTile)
            {
                topTile.selectable = true;
            }
            color = Color.blue;
        }
        else if (attackSelectable)
        {
            if (TopTile)
            {
                topTile.attackSelectable = true;
            }
            color = Color.red;
        }
        else
        {
            color = Color.white;
        }

        if (tilesOnTop > 0)
        {           
            TopTile.GetComponent<Renderer>().material.color = color;
        }
        else
        {
            GetComponent<Renderer>().material.color = color;
        }
    }
    public void Reset() //On reset les tiles pour la prochaine recherche
    {
        walkable = true;
        if (TopTile)
        {
            TopTile.selectable = false;
        }
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

    public void CheckTile(Direction direction, float characterJumpHeight) //On regarde si ya une tile dans la direction demandee, si y'en a, on check si elle est walkable ou used par un character selon les besoins (attack ou mouvement)
    {
        highestTileReachableFromThisTile = TilesOnTop + characterJumpHeight;
        lowestTileReachableFromThisTile = TilesOnTop - characterJumpHeight;
        BaseTile tile;
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

    bool IsTileAvailable(BaseTile tile)
    {
        if (forAttack)
        {
            return (tile); //pt qu'on veut pas que les case not walkable ou avec obstacles soit selectable, a voir.
        }
        return (tile && tile.walkable && !tile.usedByCharacter && tile.TilesOnTop <= highestTileReachableFromThisTile && tile.tilesOnTop >= lowestTileReachableFromThisTile); //plus tard on pourra pt walk throught les coequipiers, ca sera a modifier
    }
}
