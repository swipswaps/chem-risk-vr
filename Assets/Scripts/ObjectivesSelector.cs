using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesSelector : MonoBehaviour
{
	public GameObject Pointer;
	public GameObject Title;
	public GameObject[] ObjectiveButtons;
	public LayerMask CatchButtonLayer;
	private Image _image;
	
	public GameObject CurrentObjectiveTitle;
	private Dictionary<string, string[]> _objectives = new Dictionary<string, string[]>();
	public GameObject[] TaskFields;
	
	public static string CurrentObjective;
	
	// ******
	// Objective variables: Use Water Bottle
	public static bool PickedUpWaterBottle = false;
	public static bool PlacedBackWaterBottle = false;
	
	// ******
	// Objective variables: Use Teleporter
	public static bool UsedTeleporter = false;

	private void Start()
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
		_objectives.Add("Force Him",
			new[] {
				"Blindfold and lock him up <3",
				"Love him with <3 gentle <3 care,",
				"Let him escape on purpose,",
				"Get completely dominated <3"
			});
		
		// After we initialize the _objectives, we pick the
		// first one and assign the title of the new objective to it
		// and update the fields in the tablet for each of its tasks.
		CurrentObjective = _objectives.ElementAt(0).Key;
		CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
		GoNextObjective();
	}

	void Update () {
		// We want the player to select a new mission on if the
		// current one is null (no current mission), so we toggle
		// the buttons for selecting objectives.
		if (CurrentObjective != null)
		{
			foreach (var button in ObjectiveButtons)
			{
				button.GetComponent<Button>().interactable = false;
			}
		} else if (CurrentObjective == null)
		{
			foreach (var button in ObjectiveButtons)
			{
				button.GetComponent<Button>().interactable = true;
			}
		}
		
		// If the player has completed the requirements to finish
		// his current objective and that objective's number is not different from any other
		// only then can we go to the next mission.
		if (PickedUpWaterBottle && PlacedBackWaterBottle && CurrentObjective == "Use Water Bottle")
		{
			TaskFields[1].GetComponentInChildren<Image>().color = Color.green;
			// We update the current objective index so that
			// we can use the new tasks for the tablet's display.
			//_currentObjectiveNumber++;
			// Once the objective is complete, we set the current objective
			// string to null, so that the player will have the option to select
			// the next objective in the selector screen.
			CurrentObjective = null;
			//GoNextObjective();
		}
		if (PickedUpWaterBottle && CurrentObjective == "Use Water Bottle")
		{
			TaskFields[0].GetComponentInChildren<Image>().color = Color.green;
		}
		if (PlacedBackWaterBottle && CurrentObjective == "Use Water Bottle")
		{
			TaskFields[1].GetComponentInChildren<Image>().color = Color.green;
		}

		if (UsedTeleporter && CurrentObjective == "Use Teleporter")
		{
			TaskFields[0].GetComponentInChildren<Image>().color = Color.green;
			CurrentObjective = null;
			//_currentObjectiveNumber++;
			//GoNextObjective();
		}
		
		Ray ray = new Ray(Pointer.transform.position, Pointer.transform.forward);
		RaycastHit hit;
		
		// If the player happens to hover over UI elements such as
		// buttons, then they will be updated here on different events.
		if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
		{
			var lookedAtButton = hit.collider.gameObject;
			bool isButtonInteractable = lookedAtButton.GetComponent<Button>().interactable;
			if (isButtonInteractable)
			{
				_image = lookedAtButton.GetComponent<Image>();
				_image.color = Color.red;

				if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
				    Input.GetKeyDown(KeyCode.K))
				{
					_image.color = Color.green;
					
					// Button functionality/behaviour on click events
					if (lookedAtButton.name == "Use Water Bottle")
					{
                        SelectObjectiveUseWaterBottle();
					} else if (lookedAtButton.name == "Use Teleporter")
					{
						SelectObjectiveUseTeleporter();
					} else if (lookedAtButton.name == "Force Him")
					{
						SelectObjectiveForceHim();
					}
				}
			}
			// If the player is holding an item and needs to return it, he must
			// be hovering over the original indicator for that item and press the
			// controller button to return the item and remove it from his hand.
		}
		else
		{
			if (_image != null)
			{
				_image.color = Color.white;
			}
		}
	}

	public void SelectObjectiveUseWaterBottle()
	{
		CurrentObjective = "Use Water Bottle";
		PickedUpWaterBottle = false;
		PlacedBackWaterBottle = false;
		GoNextObjective();
	}

	public void SelectObjectiveUseTeleporter()
	{
		CurrentObjective = "Use Teleporter";
		UsedTeleporter = false;
		GoNextObjective();
	}

	public void SelectObjectiveForceHim()
	{
		CurrentObjective = "Force Him";
		GoNextObjective();
	}
	
    public void GoNextObjective()
    {
        for (int j = 0; j < TaskFields.Length; j++)
        {
            TaskFields[j].GetComponentInChildren<Image>().color = Color.white;
            TaskFields[j].GetComponentInChildren<Text>().text = "";
        }
		
        CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
	    Title.GetComponent<Text>().text = "Currently: " + CurrentObjective;
	    
	    // *********************************************************************
	    // This will be useful if we want to give the player one objective
	    // after another in a deterministic approach compared to the current one
	    // where the player can select his next objective.
	    /*
	    // Initialize the first objective without incrementing
	    // to the second one when the game starts.
	    if (_currentObjectiveNumber > 0)
	    {
	        for (int i = 0; i < _objectives.Count; i++)
	        {
	            if (i == _currentObjectiveNumber)
	            {
		
	                //Debug.Log(CurrentObjective);

	                // Elementat().Key -> returns the value of the key
	                // in the given index of the dictionary.
	                CurrentObjective = _objectives.ElementAt(i).Key;
	                // Update the title in the tablet with the new objective above.
	                CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
	            }
	        }
	    }
	    */
	    
        foreach (var objective in _objectives)
        {
            if (objective.Key == CurrentObjective)
            {
                for (int i = 0; i < objective.Value.Length; i++)
                {
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
    }
}
