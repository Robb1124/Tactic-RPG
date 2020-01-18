using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionPanel : MonoBehaviour
{
    [SerializeField] GameObject actionButtonPrefab;
    [SerializeField] GameObject actionDescriptionPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActionList()
    {
        ClearActionList();
        List<Ability> abilitiesList = BattleManager.Instance.ActiveCharacter.learnedAbilities;
        for (int i = 0; i < abilitiesList.Count; i++)
        {
            GameObject actionButton = Instantiate(actionButtonPrefab, this.transform);          
            actionButton.GetComponentInChildren<Text>().text = abilitiesList[i].abilityName;
            actionButton.GetComponent<ActionButton>().SetIndexAndButtonEvent(i, abilitiesList[i].abilityDescription, this, actionDescriptionPanel);
        }       
    }

    public void ClearActionList()
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {            
            Destroy(transform.GetChild(i).gameObject);
        }        
    }
}
