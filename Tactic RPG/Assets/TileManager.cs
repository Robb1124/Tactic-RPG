using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] BaseTile[,] tiles;
    BattleManager battleManager;

    public BaseTile[,] Tiles { get => tiles; set => tiles = value; }
    public int Width { get => width; set => width = value; } //on doit specifier le width and height de notre map pour que le manager les recense bien
    public int Height { get => height; set => height = value; }
    public int DistanceGoal { get => distanceGoal; set => distanceGoal = value; }

    int distanceGoal = 0;
    // Start is called before the first frame update
    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        Tiles = new BaseTile[Width, Height];
        FeedTiles();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FeedTiles() //va chercher ses tiles selon le width et height de la scene, et ensuite les tiles vont regarder si des actors y sont present, si oui, on register le tile index dans le actor script.
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Collider[] colliders = Physics.OverlapSphere(new Vector3((0.5f - Width / 2) + i, 0, (-0.5f + Height / 2) - j), 0.2f); //on fait un raycast en sphere aux positions ou il doit y avoir des tiles pour les recenser
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.tag == "Tile")
                    {
                        Tiles[i, j] = collider.gameObject.GetComponent<BaseTile>();
                        //tiles[i, j].GetComponent<Renderer>().material.color = Color.red; //Line qui servait a debugger et voir les tiles faisant partie du grid selon le width and height.
                        Tiles[i, j].SetTileIndex(new TileIndex(i, j));
                        Tiles[i, j].CheckIfCharacterOnTile(); //Raycast a savoir si un Actor est present sur le tile, si oui, la tile est occupied et on register la position dans le Actor script.
                        Tiles[i, j].CalculateTilesHeight(); //raycast a savoir combien de tiles sont sur la base tile, pour connaitre la hauteur du monde.
                        break;
                    }
                }
            }
        }
        //Le tile manager a faite sa job, on est pret a start la battle
        battleManager.StartBattle();
    }

    public void ComputeAdjencyList(float jumpOrAttackHeight, bool forAttack)
    {       
        foreach (BaseTile tile in Tiles)
        {
            tile.FindNeighbors(jumpOrAttackHeight, forAttack);
        }
        
    }
}
