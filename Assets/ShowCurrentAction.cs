using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowCurrentAction : MonoBehaviour
{
    Text ourActionText;
    GoapAgent ourAgent;
    // Use this for initialization

    private void Awake()
    {
        ourAgent = gameObject.GetComponentInParent<GoapAgent>();
    }

    void Start()
    {
        ourActionText = GetComponent<Text>();
    }

    void ChangeTextOnActionChange(GoapAction action)
    {
        string actionToString = action.ToString();
        ourActionText.text = actionToString; 
    }

    private void OnEnable()
    {
        ourAgent.ActionChanged += this.ChangeTextOnActionChange;
    }

    private void OnDisable()
    {
        ourAgent.ActionChanged -= this.ChangeTextOnActionChange; 
    }

    void Update()
    {
        
    }
}
