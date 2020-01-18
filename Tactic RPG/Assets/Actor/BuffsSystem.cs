using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public BuffType buffType;
    public int turnRemaining;

    public Buff(BuffType buffType, int numberOfTurns)
    {
        this.buffType = buffType;
        this.turnRemaining = numberOfTurns;
    }
}

[System.Serializable]
public class StatsChange
{
    public StatsAffected[] affectedStats; //des arrays au cas ou un buff affecterait plusieurs stats : ie: Un stun pourrait reduire le move range et les degats
    public int[] affectedByValue;
    public float[] affectedByPercentage;

    public StatsChange(StatsAffected[] affectedStats, int[] affectedByValue, float[] affectedByPercentage)
    {
        this.affectedStats = affectedStats;
        this.affectedByValue = affectedByValue;
        this.affectedByPercentage = affectedByPercentage;
    }
}
public enum StatsAffected
{
    HP,
    DMG,
    MOVE,
    JUMP
}

public class BuffsSystem : MonoBehaviour
{
    /// <summary>
    /// Liste des buffs presentement actifs sur le character
    /// </summary>
    [SerializeField] List<Buff> currentActiveBuffs = new List<Buff>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This method will remove a specific buff, if it exists, from the character active buffs.
    /// </summary>
    /// <param name="buffType"> The type of buff you want to remove</param>
    public void RemoveBuff(BuffType buffType)
    {
        Buff buffToRemove = currentActiveBuffs.SingleOrDefault(b => b.buffType == buffType);
        if (buffToRemove != null)
        {
            currentActiveBuffs.Remove(buffToRemove);
            RemoveOrAddEffect(buffType, false);
        }
    }

    /// <summary>
    /// Adds a buff to the list of current active buff of this character.
    /// </summary>
    /// <param name="buffType"> The type of buff you want to add</param>
    /// <param name="numberOfTurns"> The number of turns for buff duration</param>
    public void AddBuff(BuffType buffType, int numberOfTurns)
    {
        Buff buffToRemove = currentActiveBuffs.SingleOrDefault(b => b.buffType == buffType);
        if(buffToRemove == null)
        {
            currentActiveBuffs.Add(new Buff(buffType, numberOfTurns));
            RemoveOrAddEffect(buffType, true);
        }       
    }

    /// <summary>
    /// Methode qui va enlever 1 tour a toutes les buffs, si un buffs arrive a 0, on le remove, on doit la call a la fin du tour du character.
    /// </summary>
    public void OnTurnChange()
    {
        foreach (Buff buff in currentActiveBuffs)
        {
            buff.turnRemaining--;           
        }
        List<Buff> temporaryList = currentActiveBuffs.FindAll(b => b.turnRemaining <= 0);
        foreach (Buff buff in temporaryList)
        {
            RemoveBuff(buff.buffType);
        }       
    }

    /// <summary>
    /// La method qui va ajouter ou enlever les bonus en lien avec les buffs.
    /// </summary>
    /// <param name="buffType">De quel type de buff on parle</param>
    /// <param name="add">True : On ajoute le bonus du buff, false : On l'enleve</param>
    void RemoveOrAddEffect(BuffType buffType, bool add)
    {
        StatsAffected[] affectedStats;
        int[] affectedByValue;
        float[] affectedByPercentage;
        StatsChange statsChange;
        switch (buffType)
        {
            case BuffType.Grunt:
                affectedStats = new StatsAffected[]{ StatsAffected.DMG, StatsAffected.HP };
                affectedByValue = new int[]{ 0, 0 };
                affectedByPercentage = new float[]{ 1.3f, 1.2f };
                statsChange = new StatsChange(affectedStats, affectedByValue, affectedByPercentage);
                if (add)
                {
                    this.GetComponent<Actor>().ReceiveOrRemoveBuffEffect(statsChange, true);
                }
                else
                {
                    this.GetComponent<Actor>().ReceiveOrRemoveBuffEffect(statsChange, false);
                }
                break;
            case BuffType.Stun:
                affectedStats = new StatsAffected[]{ StatsAffected.DMG };
                affectedByValue = new int[]{ 0 };
                affectedByPercentage = new float[]{ 0.7f };
                statsChange = new StatsChange(affectedStats, affectedByValue, affectedByPercentage);
                if (add)
                {
                    this.GetComponent<Actor>().ReceiveOrRemoveBuffEffect(statsChange, true);

                }
                else
                {
                    this.GetComponent<Actor>().ReceiveOrRemoveBuffEffect(statsChange, false);
                }
                break;
        }
    }
}
