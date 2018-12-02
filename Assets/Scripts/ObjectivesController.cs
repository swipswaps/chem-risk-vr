using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesController : MonoBehaviour
{
    public GameObject CurrentObjectiveTitle;

    private int _currentObjectiveNumber = 0;
    private Dictionary<string, string[]> _objectives = new Dictionary<string, string[]>();
    public GameObject[] TaskFields;
	
    void Start ()
    {
    }
	
    void Update () {
    }

    public void GoNextObjective()
    {
        /*
        // Reset the field values in the tablet's tasks so that
        // they can be updated with new ones if they happen to be left out
        // from the previous objective.
        for (int j = 0; j < TaskFields.Length; j++)
        {
            TaskFields[j].GetComponentInChildren<Image>().color = Color.white;
            TaskFields[j].GetComponentInChildren<Text>().text = "";
        }
		
        CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
        
        foreach (var objective in _objectives)
        {
            // If the objective we are looking for next is
            // equal to the new current objective...
            if (objective.Key == CurrentObjective)
            {
                //Debug.Log(objective.Key + " - " + CurrentObjective);
                // Then we want to look for every task in that objective and...
                for (int i = 0; i < objective.Value.Length; i++)
                {
                    // add the available tasks from that new current objective
                    // until there are no more left to add in the tablet display fields.
                    for (int j = 0; j < TaskFields.Length; j++)
                    {
                        if (i == j)
                        {
                            TaskFields[j].GetComponentInChildren<Text>().text = objective.Value[i];
                        }
                    }
                }
            }
        }
        */
    }
}