using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Actor : MonoBehaviour
{
    [SerializeField] TileIndex characterTileIndex;
    [SerializeField] int moveRange = 3;
    [SerializeField] int jumpHeight = 1;
    [SerializeField] int attackRange = 1;
    [SerializeField] int attackHeight = 0;
    [SerializeField] int attackDamage = 10;
    [SerializeField] int maxHp = 30;
    int currentHp;
    Tile currentTile;
    Tile targetTile;
    protected Tile clickedTile;
    protected bool canChooseWhereToMove = false;
    protected bool canChooseWhereToAttack = false;

    protected bool isMoving = false;
    public TileIndex CharacterTileIndex { get => characterTileIndex; set => characterTileIndex = value; }
    TileManager tileManager;
    List<Tile> selectableTiles = new List<Tile>();
    Stack<Tile> path = new Stack<Tile>();
    BattleManager battleManager;
    Animator animator;

    bool resetTilesCalled = false; //pour s'assurer qu'on reset pas les tiles every frame pour aucune raison.

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHp = maxHp;
        animator = GetComponent<Animator>();
        tileManager = FindObjectOfType<TileManager>();
        battleManager = FindObjectOfType<BattleManager>();
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if(isMoving)
        {
            Move();
        }
    }

    public void DisplayMoveRange()
    {
        tileManager.ComputeAdjencyList(attackHeight, false); //Toutes les tiles prennent conscience de leurs neighbors
        Queue<Tile> process = new Queue<Tile>(); //On cree une file dans lequel on va mettre les tiles a process. Une file = First in first out.
        currentTile = tileManager.Tiles[characterTileIndex.X, characterTileIndex.Z]; //On prend la tile du character pour savoir a partir de ou on bouge
        currentTile.current = true; //On notify la tile que cest la current tile (pour l'instant sert a colorer differement la tile sous le personnage qui va bouger
        process.Enqueue(currentTile); //On met notre tile dans la process queue
        currentTile.visited = true; //On indique que cette tile est visite pour pas qu'on la process 2 fois
        while (process.Count > 0) //Tant que la queue n'est pas vide, on continue a process
        {
            Tile t = process.Dequeue(); //On enleve le premier de la queue en en gardant une reference
            selectableTiles.Add(t); //On ajoute cette tile dans notre liste de tile selectable (pour qu'on puisse la cliquer plus tard pour bouger)
            t.selectable = true; //On indique que la tile est selectable pour qu'elle change de couleur
            if (t.distance < moveRange) //On s'assure de respecter notre move range (distance est defaulted a 0, donc la premier tile processed a 0 de distance
            {
                foreach (Tile tile in t.adjacencyList) //on passe tous les neighbors de la tile, parce que si il nous reste du move range, on peut aller sur tous ses neighbors
                {
                    if (!tile.visited) //On ne veut pas process 2 fois la meme tile, donc on skip les visited.
                    {
                        tile.parent = t; //On assigne le parent de la tile (va servir a construire le path en suivant la lignee de parent a partir de la tile choisie pour se deplacer.
                        tile.visited = true;
                        tile.distance = 1 + t.distance; //On assigne la distance a cette tile
                        process.Enqueue(tile); //Toutes les tiles voisines de la tile qui se fait process vont devoir etre process par la suite si la distance le permet.
                    }
                }
            }
        }
        currentTile.selectable = false;
        canChooseWhereToMove = true;         
    }

    public void DisplayAttackRange()
    {
        tileManager.ComputeAdjencyList(jumpHeight, true); //Toutes les tiles prennent conscience de leurs neighbors
        Queue<Tile> process = new Queue<Tile>(); //On cree une file dans lequel on va mettre les tiles a process. Une file = First in first out.
        currentTile = tileManager.Tiles[characterTileIndex.X, characterTileIndex.Z]; //On prend la tile du character pour savoir a partir de ou on attaque
        currentTile.current = true; //On notify la tile que cest la current tile (pour l'instant sert a colorer differement la tile sous le personnage qui va attaquer)
        process.Enqueue(currentTile); //On met notre tile dans la process queue
        currentTile.visited = true; //On indique que cette tile est visite pour pas qu'on la process 2 fois
        while (process.Count > 0) //Tant que la queue n'est pas vide, on continue a process
        {
            Tile t = process.Dequeue(); //On enleve le premier de la queue en en gardant une reference
            selectableTiles.Add(t); //On ajoute cette tile dans notre liste de tile selectable (pour qu'on puisse la cliquer plus tard pour attaquer)
            t.attackSelectable = true; //On indique que la tile est selectable pour une attaque (on la color red)
            if (t.distance < attackRange) //On s'assure de respecter notre attack range (distance est defaulted a 0, donc la premier tile processed a 0 de distance)
            {
                foreach (Tile tile in t.adjacencyList) //on passe tous les neighbors de la tile, parce que si il nous reste du attack range, on peut aller sur tous ses neighbors
                {
                    if (!tile.visited) //On ne veut pas process 2 fois la meme tile, donc on skip les visited.
                    {
                        //pas besoin du parent pour l'attaque, on veut juste savoir quel case on peut hit, pas le chemin pour s'y rendre.
                        tile.visited = true;
                        tile.distance = 1 + t.distance; //On assigne la distance a cette tile
                        process.Enqueue(tile); //Toutes les tiles voisines de la tile qui se fait process vont devoir etre process par la suite si la distance le permet.
                    }
                }
            }
        }
        currentTile.attackSelectable = false;
        canChooseWhereToAttack = true;
    }

    public void RecreatePathToTargetTile(Tile tile)
    {
        path.Clear();
        tile.target = true;      

        Tile next = tile;
        while(next != null) // quand next est null cest qu'on est arriver au starting tile.
        {
            path.Push(next); //on met la next tile dans la stack, first in last out, pour creer le chemin que le personnage va prendre jusqu'a la target tile.
            next = next.parent;
        }
        isMoving = true;
    }

    public void Move()
    {
        tileManager.Tiles[characterTileIndex.X, characterTileIndex.Z].usedByCharacter = false;

        if (!resetTilesCalled)//pour pas reset a chaque fois que move est called.
        {
            resetTilesCalled = true;
            RemoveSelectableTiles();
        }

        if (path.Count > 0) //si ya des tiles dans notre pathing, on bouge.
        {
            Tile t = t = path.Peek();
            Vector3 target = t.transform.position;
            target.y += 0.5f; //Pour rester au niveau du plancher.
            
            if(Vector3.Distance(transform.position, target) >= 0.38f)
            {
                if(path.Count == 1)
                {
                    GetComponent<AICharacterControl>().SetTarget(target, () => transform.position = target); //Quand cest la derniere tile a move to, on envoie une ligne de code a executer quand la target est reached.
                    t.usedByCharacter = true;
                    t.CharacterOnTile = this;
                    characterTileIndex = t.index;
                }
                else
                {
                    GetComponent<AICharacterControl>().SetTarget(target); //on se dirige vers la tile sur le dessus de la pile
                }
            }
            else
            {
                path.Pop(); // une fois la tile sur le dessus de la pile atteinte, on l'enleve de la pile et on en prend une autre
            }
        }
        else
        {
            isMoving = false;
            battleManager.DoneWithMovement(); //termine la phase de mouvement du player
            resetTilesCalled = false;
        }
    }

    protected void RemoveSelectableTiles()
    {
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        StartCoroutine(ResetClickedTileAfterDelay()); //Pour que notre selection soit afficher un peu plus longtemps
        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();           
        }

        selectableTiles.Clear();
    }

    IEnumerator ResetClickedTileAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        clickedTile.target = false;
    }

    protected void AttackTile(Tile targetTile)
    {
        RemoveSelectableTiles(); //fait juste reseter le choix de tile pour changer les couleurs et qu'on puisse pas spam click
        this.targetTile = targetTile; 
        targetTile.target = true;
        transform.LookAt(targetTile.transform.position);
        animator.SetTrigger("Attack");
    }

    public void Hit() //le hit est un animation event qui est call quand l'attack animation semble faire impact.
    {
        if (targetTile.CharacterOnTile)
        {
            targetTile.CharacterOnTile.TakeDamage(attackDamage);
        }
        battleManager.DoneWithAttack();
    }

    public void TakeDamage(int dmgAmount)
    {       
        currentHp -= dmgAmount;
        print(currentHp);
        if(currentHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
