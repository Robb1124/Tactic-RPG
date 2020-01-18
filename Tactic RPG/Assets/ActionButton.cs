using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    int abilityIndex;
    string abilityDescription = "";
    GameObject actionDescriptionPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIndexAndButtonEvent(int abilityIndex, string abilityDescription, ActionPanel actionPanel, GameObject actionDescriptionPanel)
    {
        this.actionDescriptionPanel = actionDescriptionPanel;
        this.abilityIndex = abilityIndex;
        this.abilityDescription = abilityDescription;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            BattleManager.Instance.ActiveCharacter.LoadAttackConfig(abilityIndex);
            actionPanel.ClearActionList();
            actionPanel.gameObject.SetActive(false);
        });      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        actionDescriptionPanel.GetComponentInChildren<Text>().text = abilityDescription;
        actionDescriptionPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        actionDescriptionPanel.SetActive(false);

    }
}
