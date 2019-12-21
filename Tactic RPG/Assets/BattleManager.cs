using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { PlayerChoose }
public class BattleManager : MonoBehaviour
{
    [SerializeField] Actor[] playerCharacters;
    Actor activeCharacter;
    [SerializeField] BattleState currentBattleState;
    TileManager tileManager;
    bool doneWithMovement = false;
    bool doneWithAttack = false;
    bool displayingRange = false;
    int playerCharacterIndex = 0;
    [SerializeField] Button moveButton;
    [SerializeField] Button attackButton;
    [SerializeField] Button cancelButton;

    public bool DoneWithMovement { get => doneWithMovement; set => doneWithMovement = value; }
    public bool DoneWithAttack { get => doneWithAttack; set => doneWithAttack = value; }
    public bool DisplayingRange { get => displayingRange; set => displayingRange = value; }

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle() //caller une fois que le tilemanager a recenser toutes les bases tiles et que les bases tiles ont pris conscience des tiles au dessus d'elles, des characters etc.
    {
        activeCharacter = playerCharacters[playerCharacterIndex]; //on commence par faire jouer le premier player pour l'instant. (defaulted a 0)
    }


    public void MoveAction() //methode called quand le bouton move est clicked (ca display le range, possible de cancel avec le cancel button)
    {
        activeCharacter.DisplayMoveRange();
        attackButton.interactable = false; //on desactive les boutons sauf le cancel et le fait de pouvoir cliquer une tile
        moveButton.interactable = false;
        cancelButton.interactable = true;
        DisplayingRange = true;
    }

    public void AttackAction() //methode called quand le bouton attack est clicked (ca display le range, possible de cancel avec le cancel button)
    {
        activeCharacter.DisplayAttackRange();
        attackButton.interactable = false; //on desactive les boutons sauf le cancel et le fait de pouvoir cliquer une tile
        moveButton.interactable = false;
        cancelButton.interactable = true;
        DisplayingRange = true;
    }

    public void CancelAction() //si on affiche le range, on le desaffiche et on permet au joueur de changer d'idee
    {
        activeCharacter.AttackIsCancelled = true;
        activeCharacter.WaitingForTargetConfirmation = false;
        activeCharacter.RemoveSelectableTiles();
        DisplayingRange = false;
        cancelButton.interactable = false;
        RefreshButtonsAndManageTurn(); //quand on cancel, on refresh les boutons a savoir lesquels peuvent etre clicked (ex : si on a deja move on le reactive pas)

        if (DisplayingRange)
        {
            activeCharacter.RemoveSelectableTiles(); //on enleve la selection de tiles                      
        }
    }

    public void RefreshButtonsAndManageTurn()
    {
        if (DoneWithAttack && DoneWithMovement) //si le active character a fait son attack et son mouvement, on passe au character suivant
        {
            SkipTurn();
        }
        attackButton.interactable = !doneWithAttack; //si on a deja attack, le button est pas interactable.
        moveButton.interactable = !doneWithMovement; //si on a deja move, le button est pas interactable.       
    }

    public void SkipTurn() //methode qui sert a changer le active character mais est aussi called si on click sur le bouton skip turn.
    {
        playerCharacterIndex = playerCharacterIndex < playerCharacters.Length - 1 ? playerCharacterIndex + 1 : 0; // si les deux actions sont faites (move + action), on passe au character suivant.
        activeCharacter = playerCharacters[playerCharacterIndex];
        DoneWithAttack = false; //on reset ses actions avant de refresh les boutons
        doneWithMovement = false;
        RefreshButtonsAndManageTurn();
    }

    public void ConfirmTargets()
    {
        activeCharacter.WaitingForTargetConfirmation = false;
    }
}
