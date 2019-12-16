using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { PlayerMove, PlayerAttack}
public class BattleManager : MonoBehaviour
{
    [SerializeField] Actor player;
    [SerializeField] BattleState currentBattleState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBattle()
    {
        currentBattleState = BattleState.PlayerMove;
        player.DisplayMoveRange();
    }

    public void DoneWithMovement() //quand le player a fini son mouvement
    {
        currentBattleState = BattleState.PlayerAttack;
        player.DisplayAttackRange();
    }

    public void DoneWithAttack() //quand le player a fini son attaque
    {
        currentBattleState = BattleState.PlayerMove;
        player.DisplayMoveRange();
    }
}
