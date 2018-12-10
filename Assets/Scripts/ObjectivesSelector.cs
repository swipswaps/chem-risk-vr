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
	private static int _currentPage = 1;
	private static bool _isNextPageAvailable = false;
	public GameObject GoNextPageButton;
	public GameObject GoPrevPageButton;
	
	// ******
	// Objective variables: Use Water Bottle
	public static bool PickedUpWaterBottle = false;
	public static bool PlacedBackWaterBottle = false;
	
	// ******
	// Objective variables: Use Teleporter
	public static bool UsedTeleporter = false;
	
	// ******
	// Objective variables: Mix Colors
	public static bool PourRedIntoTube = false;
	public static bool PourBlueIntoTube = false;
	public static bool PourYellowIntoTube = false;
	public static bool MixRedAndYellow = false;
	public static bool MixRedAndBlue = false;
	public static bool MixBlueAndYellow = false;

	private void Start()
	{
		// First Objective and its task.
		_objectives.Add("Use Water Bottle", // Objective's title
			new[] {
				"Pick up water bottle", // Objective task number one
				"Place back water bottle" // Objective task number two...
			});
		/*_objectives.Add("Use Teleporter",
			new[]
			{
				"Index click on a teleporter"
			});*/
		_objectives.Add("Mix Colors",
			new[] {
				"Pour color red into a test tube",
				"Pour color blue into a test tube",
				"Pour color yellow into a test tube",
				"Mix colors red and yellow in a beaker to make orange",
				"Mix colors red and blue in a beaker to make purple",
				"Mix colors blue and yellow in a beaker to make green"
			});
		
		// After we initialize the _objectives, we pick the
		// first one and assign the title of the new objective to it
		// and update the fields in the tablet for each of its tasks.
		CurrentObjective = _objectives.ElementAt(0).Key;
		CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
		GoNextObjective();
		
		for (int i = 2; i < 12; i++)
		{
			TaskFields[i].SetActive(false);
		}
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
		} else if (CurrentObjective == null && HandinController._isObjectiveHandedIn)
		{
			foreach (var button in ObjectiveButtons)
			{
				button.GetComponent<Button>().interactable = true;
			}
		}
		
		// If the player has completed the requirements to finish
		// his current objective and that objective's number is not different from any other
		// only then can we go to the next mission.
		if (PickedUpWaterBottle && PlacedBackWaterBottle &&
		    CurrentObjective == "Use Water Bottle" && HandinController._isObjectiveHandedIn)
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

		if (UsedTeleporter && CurrentObjective == "Mix Colors"
		                   && HandinController._isObjectiveHandedIn)
		{
			TaskFields[0].GetComponentInChildren<Image>().color = Color.green;
			CurrentObjective = null;
			//_currentObjectiveNumber++;
			//GoNextObjective();
		}

		if (CurrentObjective == "Mix Colors")
		{
			if (PourRedIntoTube) { TaskFields[0].GetComponentInChildren<Image>().color = Color.green; }
			if (PourBlueIntoTube) { TaskFields[1].GetComponentInChildren<Image>().color = Color.green; }
			if (PourYellowIntoTube) { TaskFields[2].GetComponentInChildren<Image>().color = Color.green; }
			if (MixRedAndYellow) { TaskFields[3].GetComponentInChildren<Image>().color = Color.green; }
			if (MixRedAndBlue) { TaskFields[4].GetComponentInChildren<Image>().color = Color.green; }
			if (MixBlueAndYellow) { TaskFields[5].GetComponentInChildren<Image>().color = Color.green; }
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
						// We dont want the player to think he should click any of these
						// buttons before he actually can be able to.
						DisableButtonsInteractivity();
					} else if (lookedAtButton.name == "Use Teleporter")
					{
						SelectObjectiveUseTeleporter();
						DisableButtonsInteractivity();
					} else if (lookedAtButton.name == "Mix Colors")
					{
						for (int i = 0; i < 4; i++)
						{
							TaskFields[i].SetActive(true);
						}
						
						SelectObjectiveMixColors();
						// We want to be able to go through the different pages of
						// this objective because it contains more than 4 tasks.
						_isNextPageAvailable = true;
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

		if (Input.GetKeyDown(KeyCode.F) && CurrentObjective == "Mix Colors" && _isNextPageAvailable)
		{
			GoForwardAPage();
		}

		if (Input.GetKeyDown(KeyCode.G) && CurrentObjective == "Mix Colors")
		{
			GoBackAPage();
		}
	}

	public void SelectObjectiveUseWaterBottle()
	{
		CurrentObjective = "Use Water Bottle";
		PickedUpWaterBottle = false;
		PlacedBackWaterBottle = false;
		// Resetting the hand-in boolean once a new objective starts.
		HandinController._isObjectiveHandedIn = false;
		GoNextObjective();
		DisableButtonsInteractivity();

		for (int i = 2; i < TaskFields.Length; i++)
		{
			TaskFields[i].SetActive(false);
		}
	}

	public void SelectObjectiveUseTeleporter()
	{
		CurrentObjective = "Use Teleporter";
		UsedTeleporter = false;
		HandinController._isObjectiveHandedIn = false;
		GoNextObjective();
		DisableButtonsInteractivity();
		
		for (int i = 1; i < TaskFields.Length; i++)
		{
			TaskFields[i].SetActive(false);
		}
	}

	public void SelectObjectiveMixColors()
	{
		CurrentObjective = "Mix Colors";
		HandinController._isObjectiveHandedIn = false;
		GoNextObjective();
		
		PourRedIntoTube = false;
		PourBlueIntoTube = false;
		PourYellowIntoTube = false;
		MixRedAndYellow = false;
		MixRedAndBlue = false;
		MixBlueAndYellow = false;
		
		for (int i = 4; i < TaskFields.Length; i++)
		{
			TaskFields[i].SetActive(false);
		}
		
		GoNextPageButton.GetComponent<Button>().interactable = true;
		GoPrevPageButton.GetComponent<Button>().interactable = false;
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

	public void GoBackAPage()
	{
		if (_currentPage > 1)
		{
			GoForwardAPage();
		}
	}

	public void GoForwardAPage()
	{
		int lastTaskInObjective = 0;
		if (_isNextPageAvailable == false)
		{
			lastTaskInObjective = _currentPage * 4 - 8;
			_currentPage--;
		}
		else
		{
			lastTaskInObjective = _currentPage * 4;
			_currentPage++;
		}
		int newTaskInObjective = _currentPage * 4;
		
		// Disable the existing enabled task fields so we start from a clean slate.
		foreach (var task in TaskFields)
		{
			task.SetActive(false);
		}
		
		// One page contains 3 elements so we multiply the page number
		// with the total elements it contains.
		for (int i = lastTaskInObjective; i < newTaskInObjective; i++)
		{
			TaskFields[i].SetActive(true);
			if (i >= TaskFields.Length - 1)
			{
				_isNextPageAvailable = false;
				// If there is no new pages available in the current objective
				// then we dont want to see the buton available to click to go to the next one.
				GoNextPageButton.GetComponent<Button>().interactable = false;
				GoPrevPageButton.GetComponent<Button>().interactable = true;
			}
			else
			{
				_isNextPageAvailable = true;
				GoNextPageButton.GetComponent<Button>().interactable = true;

				if (i <= 4)
				{
					GoPrevPageButton.GetComponent<Button>().interactable = false;
				}
				else
				{
					GoPrevPageButton.GetComponent<Button>().interactable = true;
				}
			}
		}
	}

	private void DisableButtonsInteractivity()
	{
		GoNextPageButton.GetComponent<Button>().interactable = false;
		GoPrevPageButton.GetComponent<Button>().interactable = false;
	}
}
