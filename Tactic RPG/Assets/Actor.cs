using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;

public class Actor : MonoBehaviour
{
    [SerializeField] TileIndex characterTileIndex;
    [SerializeField] int moveRange = 3;
    [SerializeField] int jumpHeight = 1;
    [SerializeField] AimPattern aimPattern; //comment les tiles selectionnable sont choisi
    [SerializeField] TargetPattern targetPattern; //Comment le spell fait effect (aoe, singletarget.. le pattern de aoe surtout)
    List<Actor> currentTargets = new List<Actor>();
    [SerializeField] int maxAttackRange = 1;
    [SerializeField] int minAttackRange = 1;
    [SerializeField] int attackSize = 1;
    [SerializeField] int attackHeight = 0;
    [SerializeField] int attackDamage = 10;
    [SerializeField] int maxHp = 30;
    int currentHp;
    BaseTile currentTile;
    BaseTile targetTile;
    protected Tile clickedTile;
    protected bool canChooseWhereToMove = false;
    protected bool canChooseWhereToAttack = false;

    //for jumps
    int lastTileHeight;
    bool jumped = false;

    protected bool isMoving = false;
    public TileIndex CharacterTileIndex { get => characterTileIndex; set => characterTileIndex = value; }
    TileManager tileManager;
    List<BaseTile> selectableTiles = new List<BaseTile>();
    Stack<BaseTile> path = new Stack<BaseTile>();
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
        tileManager.ComputeAdjencyList(jumpHeight, false); //Toutes les tiles prennent conscience de leurs neighbors
        Queue<BaseTile> process = new Queue<BaseTile>(); //On cree une file dans lequel on va mettre les tiles a process. Une file = First in first out.
        currentTile = tileManager.Tiles[characterTileIndex.X, characterTileIndex.Z]; //On prend la tile du character pour savoir a partir de ou on bouge
        currentTile.current = true; //On notify la tile que cest la current tile (pour l'instant sert a colorer differement la tile sous le personnage qui va bouger
        process.Enqueue(currentTile); //On met notre tile dans la process queue
        currentTile.visited = true; //On indique que cette tile est visite pour pas qu'on la process 2 fois
        while (process.Count > 0) //Tant que la queue n'est pas vide, on continue a process
        {
            BaseTile t = process.Dequeue(); //On enleve le premier de la queue en en gardant une reference
            selectableTiles.Add(t); //On ajoute cette tile dans notre liste de tile selectable (pour qu'on puisse la cliquer plus tard pour bouger)                       
            t.selectable = true; //On indique que la tile est selectable pour qu'elle change de couleur
            
            if (t.distance < moveRange) //On s'assure de respecter notre move range (distance est defaulted a 0, donc la premier tile processed a 0 de distance
            {
                foreach (BaseTile tile in t.adjacencyList) //on passe tous les neighbors de la tile, parce que si il nous reste du move range, on peut aller sur tous ses neighbors
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
        lastTileHeight = currentTile.TilesOnTop; //va servir quand on va bouger, savoir on est a quel height au depart
    }

    public void DisplayAttackRange()
    {
        currentTile = tileManager.Tiles[characterTileIndex.X, characterTileIndex.Z]; //On prend la tile du character pour savoir a partir de ou on attaque
        currentTile.current = true; //On notify la tile que cest la current tile (pour l'instant sert a colorer differement la tile sous le personnage qui va attaquer)
        tileManager.ComputeAdjencyList(attackHeight, true); //Toutes les tiles prennent conscience de leurs neighbors
        switch (aimPattern)
        {
            case AimPattern.Standard:
                Queue<BaseTile> process = new Queue<BaseTile>(); //On cree une file dans lequel on va mettre les tiles a process. Une file = First in first out.
                process.Enqueue(currentTile); //On met notre tile dans la process queue
                currentTile.visited = true; //On indique que cette tile est visite pour pas qu'on la process 2 fois
                while (process.Count > 0) //Tant que la queue n'est pas vide, on continue a process
                {
                    BaseTile t = process.Dequeue(); //On enleve le premier de la queue en en gardant une reference
                    selectableTiles.Add(t); //On ajoute cette tile dans notre liste de tile selectable (pour qu'on puisse la cliquer plus tard pour attaquer)

                    if (t.distance > minAttackRange - 1)
                    {
                        t.attackSelectable = true; //On indique que la tile est selectable pour une attaque (on la color red)
                    }
                    if (t.distance < maxAttackRange) //On s'assure de respecter notre attack range (distance est defaulted a 0, donc la premier tile processed a 0 de distance)
                    {
                        foreach (BaseTile tile in t.adjacencyList) //on passe tous les neighbors de la tile, parce que si il nous reste du attack range, on peut aller sur tous ses neighbors
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
                break;
            case AimPattern.Around:
                for (int i = -maxAttackRange; i <= maxAttackRange; i++)
                {
                    for (int j = -maxAttackRange; j <= maxAttackRange; j++)
                    {
                        if(Mathf.Abs(i) >= minAttackRange || Mathf.Abs(j) >= minAttackRange)
                        {
                            BaseTile t = tileManager.RequestTile(currentTile.index.X + i, currentTile.index.Z + j);
                            if (t && Mathf.Abs(currentTile.TilesOnTop - t.TilesOnTop) <= attackHeight)
                            {
                                t.attackSelectable = true;
                                selectableTiles.Add(t);
                            }
                        }                                               
                    }
                }
                break;
        }                     
        currentTile.attackSelectable = false;
        canChooseWhereToAttack = true;
    }

    public void RecreatePathToTargetTile(Tile tile)
    {
        path.Clear(); //on clear le stack path pour partir a neuf
        BaseTile next;
        if (tile is HeightTile) //si cest une height tile qu'on a clicked, on va referencer la base tile de cette tile (le systeme fonctionne a partir des base tile)
        {
            ((HeightTile)tile).BaseTile.target = true;
            next = ((HeightTile)tile).BaseTile;
        } 
        else //sinon, on presuppose que cest une basetile est on reference la tile elle meme
        {
            tile.target = true;
            next = (BaseTile)tile;
        }       
        while (next != null) // quand next est null cest qu'on est arriver au starting tile.
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
            BaseTile t = path.Peek();            
            Vector3 target = new Vector3(t.transform.position.x, t.transform.position.y + (0.5f * t.TilesOnTop), t.transform.position.z); //on compute le height pour que le personnage se dirige se les tiles du dessus et non la base tile.
            target.y += 0.25f; //Pour rester au niveau du plancher.
            int heightDifference = t.TilesOnTop - lastTileHeight;
            if (heightDifference > 1 || heightDifference < -1) //jump
            {
                GetComponent<AICharacterControl>().SetTarget(target);
                GetComponent<NavMeshAgent>().Warp(target); //ca ca le teleporte sur la tile plus haute ou plus basse. mais ca serait ici qu'on ferait l'animation de jumping quand on voudra enlever le teleporting. (ou une animation de teleporting ex: mage)
                jumped = true;                
            }
            if (Vector3.Distance(transform.position, target) >= 0.38f && !jumped)
            {                
                if(path.Count == 1)
                {
                    GetComponent<AICharacterControl>().SetTarget(target, () => transform.position = target); //Quand cest la derniere tile a move to, on envoie une ligne de code a executer quand la target est reached.                    
                }
                else
                {
                    GetComponent<AICharacterControl>().SetTarget(target); //on se dirige vers la tile sur le dessus de la pile
                }                
            }
            else
            {
                lastTileHeight = t.TilesOnTop;
                if(path.Count == 1)
                {
                    t.usedByCharacter = true;
                    t.CharacterOnTile = this;
                    characterTileIndex = t.index;
                }
                path.Pop(); // une fois la tile sur le dessus de la pile atteinte, on l'enleve de la pile et on en prend une autre
                jumped = false;
            }
        }
        else
        {
            isMoving = false;
            battleManager.DoneWithMovement = true; //termine la phase de mouvement du player
            battleManager.RefreshButtonsAndManageTurn(); 
            resetTilesCalled = false;
        }
    }

    public void RemoveSelectableTiles()
    {
        battleManager.DisplayingRange = false;
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        StartCoroutine(ResetClickedTileAfterDelay()); //Pour que notre selection soit afficher un peu plus longtemps
        foreach (BaseTile tile in selectableTiles)
        {
            tile.Reset();           
        }
        selectableTiles.Clear();
    }

    IEnumerator ResetClickedTileAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        if(clickedTile != null)
        {
            if (clickedTile is BaseTile)
            {
                clickedTile.target = false;
            }
            else
            {
                ((HeightTile)clickedTile).BaseTile.target = false;
            }
        }       
    }

    protected void AttackTile(Tile targetTile)
    {
        currentTargets.Clear();
        switch (targetPattern)
        {
            case TargetPattern.SingleTarget:
                if (targetTile is HeightTile)
                {
                    this.targetTile = ((HeightTile)targetTile).BaseTile;
                    ((HeightTile)targetTile).BaseTile.target = true;
                }
                else
                {
                    this.targetTile = (BaseTile)targetTile;
                }
                currentTargets.Add(this.targetTile.CharacterOnTile);
                break;
            case TargetPattern.AllTargetInAim:
                foreach (BaseTile tile in selectableTiles)
                {
                    currentTargets.Add(tile.CharacterOnTile);
                }
                break;
        }
        RemoveSelectableTiles(); //fait juste reseter le choix de tile pour changer les couleurs et qu'on puisse pas spam click        
        transform.LookAt(targetTile.transform.position);
        animator.SetTrigger("Attack");
    }

    public void Hit() //le hit est un animation event qui est call quand l'attack animation semble faire impact.
    {
        for (int i = 0; i < currentTargets.Count; i++)
        {
            if (currentTargets[i] != null && currentTargets[i] != this)
            {
                currentTargets[i].TakeDamage(attackDamage);
                print(currentTargets[i].name);
                print(attackDamage);
            }
        }
        battleManager.DoneWithAttack = true; //termine phase d'attack du character
        battleManager.RefreshButtonsAndManageTurn();
    }

    public void TakeDamage(int dmgAmount)
    {       
        currentHp -= dmgAmount;
        //print(currentHp);
        if(currentHp <= 0)
        {
            tileManager.Tiles[characterTileIndex.X, characterTileIndex.Z].usedByCharacter = false; //quand on creve, la tile est pu used 
            Destroy(this.gameObject);
        }
    }
}
