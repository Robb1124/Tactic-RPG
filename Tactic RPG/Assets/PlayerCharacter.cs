using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Actor
{
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (canChooseWhereToMove || canChooseWhereToAttack) //On check pour un click seulement si cet actor est dans son move action ou son attack action
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //On part un ray a partir de la camera jusqu'au point clique dans l'ecran

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, 1 << 8))  //On store l'information du ray dans le hit. max distance de 100 metres. 
                                                                 //On ignore TOUS LES LAYERS sauf le layer Tile comme ca on peut raycast au travers des players et obstacles. 
                {
                    if (hit.collider.GetComponent<Tile>()) //Si on hit un tile
                    {
                        if(clickedTile != null)
                            clickedTile.target = false;
                        clickedTile = hit.collider.GetComponent<Tile>(); //on store la clicked tile                        
                        if (clickedTile.selectable || clickedTile.attackSelectable) //si on a actually click sur une tile qui faisait partie des choix de mouvements ou d'attaque
                        {
                            if (canChooseWhereToMove)
                            {
                                //move to its position.
                                canChooseWhereToMove = false;
                                RecreatePathToTargetTile(clickedTile);
                            }
                            else if (canChooseWhereToAttack)
                            {
                                canChooseWhereToAttack = false;
                                AttackTile(clickedTile);
                            }                            
                        }
                    }
                }
            }
        }
    }
}
