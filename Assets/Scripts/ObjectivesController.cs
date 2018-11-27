using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesController : MonoBehaviour
{
    public GameObject CurrentObjectiveTitle;
    private string _currentObjective;

    private int _currentObjectiveNumber = 0;
    private Dictionary<string, string[]> _objectives = new Dictionary<string, string[]>();
    public GameObject[] TaskFields;
	
    // ******
    // Objective variables: Use Water Bottle
    public static bool PickedUpWaterBottle = false;
    public static bool PlacedBackWaterBottle = false;
	
    // ******
    // Objective variables: Use Teleporter
    public static bool UsedTeleporter = false;
	
    // Use this for initialization
    void Start ()
    {
        // First Objective and its task.
        _objectives.Add("Use Water Bottle", // Objective's title
            new[] {
                "Pick up water bottle", // Objective task number one
                "Place back water bottle" // Objective task number two...
            });
        _objectives.Add("Use Teleporter",
            new[]
            {
                "Index click on a teleporter"
            });
        _objectives.Add("Force him",
            new[] {
                "Blindfold and lock him up!",
                "Love him with gentle care",
                "Let me escape on purpose",
                "Get completely dominated!"
            });
		
        // After we initialize the _objectives, we pick the
        // first one and assign the title of the new objective to it
        // and update the fields in the tablet for each of its tasks.
        _currentObjective = _objectives.ElementAt(0).Key;
        CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + _currentObjective;
        GoNextObjective();
    }
	
    void Update () {
        // If the player has completed the requirements to finish
        // his current objective and that objective's number is not different from any other
        // only then can we go to the next mission.
        if (PickedUpWaterBottle && PlacedBackWaterBottle && _currentObjectiveNumber == 0)
        {
            TaskFields[1].GetComponentInChildren<Image>().color = Color.green;
            // We update the current objective index so that
            // we can use the new tasks for the tablet's display.
            _currentObjectiveNumber++;
            GoNextObjective();
        }
        if (PickedUpWaterBottle && _currentObjectiveNumber == 0)
        {
            TaskFields[0].GetComponentInChildren<Image>().color = Color.green;
        }
        if (PlacedBackWaterBottle && _currentObjectiveNumber == 0)
        {
            TaskFields[1].GetComponentInChildren<Image>().color = Color.green;
        }

        if (UsedTeleporter && _currentObjectiveNumber == 1)
        {
            TaskFields[0].GetComponentInChildren<Image>().color = Color.green;
            _currentObjectiveNumber++;
            GoNextObjective();
        }
    }

    private void GoNextObjective()
    {
        // Reset the field values in the tablet's tasks so that
        // they can be updated with new ones if they happen to be left out
        // from the previous objective.
        for (int j = 0; j < TaskFields.Length; j++)
        {
            TaskFields[j].GetComponentInChildren<Image>().color = Color.white;
            TaskFields[j].GetComponentInChildren<Text>().text = "";
            //Debug.Log("Reset a field");
        }
		
        // Initialize the first objective without incrementing
        // to the second one when the game starts.
        if (_currentObjectiveNumber > 0)
        {
            for (int i = 0; i < _objectives.Count; i++)
            {
                if (i == _currentObjectiveNumber)
                {
		
                    //Debug.Log(_currentObjective);

                    // Elementat().Key -> returns the value of the key
                    // in the given index of the dictionary.
                    _currentObjective = _objectives.ElementAt(i).Key;
                    // Update the title in the tablet with the new objective above.
                    CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + _currentObjective;
                }
            }
        }
        foreach (var objective in _objectives)
        {
            // If the objective we are looking for next i
            // equal to the new current objective...
            if (objective.Key == _currentObjective)
            {
                //Debug.Log(objective.Key + " - " + _currentObjective);
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
                            //Debug.Log(objective.Value[i]);
                        }
                    }
                }
            }
        }
    }
}