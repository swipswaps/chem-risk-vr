using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesSelector : MonoBehaviour
{
    private GameObject _player;
    public GameObject LabEquipment;
    private AudioSource _playerAudioSource;
    public GameObject Pointer;
    public GameObject Title;
    public GameObject[] ObjectiveButtons;
    public LayerMask CatchButtonLayer;
    private Image _image;
	
    public GameObject CurrentObjectiveTitle;
    public Text SafetyGuidelinesBoardText;
    private Dictionary<string, string[]> _objectives = new Dictionary<string, string[]>();
    public GameObject[] TaskFields;

    public GameObject CurrentObjectiveText;
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

    // Fields responsible for the indicator mechanic on the tablet.
    [SerializeField] private GameObject _labCoatIndicator;
    private MeshRenderer _labCoatRenderer;
    [SerializeField] private GameObject _labGlassesIndicator;
    private MeshRenderer _labGlassesRenderer;
    [SerializeField] private GameObject _labGlovesIndicator;
    private MeshRenderer _labGlovesRenderer;
    [SerializeField] private Material _normalIndicator;
    [SerializeField] private Material _redFlashIndicator;
    [SerializeField] private Material _greenFlashIndicator;
	
    public GameObject FadeTransitioner;
    public static bool CanOpenDoor = false;
    public static bool  CanWearEquipment = false;
    public GameObject TextObject;

    private void Start()
    {
        FadeTransitioner.SetActive(false);


        _player = GameObject.FindGameObjectWithTag("Player");

        _labCoatRenderer = _labCoatIndicator.GetComponent<MeshRenderer>();
        _labGlassesRenderer = _labGlassesIndicator.GetComponent<MeshRenderer>();
        _labGlovesRenderer = _labGlovesIndicator.GetComponent<MeshRenderer>();
        _playerAudioSource = _player.GetComponent<AudioSource>();
		
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

        /*PointerController.IsWearingCoat = true;
        PointerController.IsWearingGlasses = true;
        PointerController.IsWearingGloves = true;
        */

        // After we initialize the _objectives, we pick the
        // first one and assign the title of the new objective to it
        // and update the fields in the tablet for each of its tasks.
        /*CurrentObjective = _objectives.ElementAt(0).Key;
        CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
        GoNextObjective();
        
        for (int i = 2; i < 12; i++)
        {
            TaskFields[i].SetActive(false);
        }*/
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
        } else if (CurrentObjective == null && HandinController.IsObjectiveHandedIn)
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
            CurrentObjective == "Use Water Bottle" && HandinController.IsObjectiveHandedIn)
        {
            TaskFields[1].GetComponentInChildren<Image>().color = Color.green;
            // We update the current objective index so that
            // we can use the new tasks for the tablet's display.
            //_currentObjectiveNumber++;
            // Once the objective is complete, we set the current objective
            // string to null, so that the player will have the option to select
            // the next objective in the selector screen.
            CurrentObjective = null;
            CurrentObjectiveText.GetComponent<Text>().text = "Select Objective...";
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
                           && HandinController.IsObjectiveHandedIn)
        {
            TaskFields[0].GetComponentInChildren<Image>().color = Color.green;
            CurrentObjective = null;
            CurrentObjectiveText.GetComponent<Text>().text = "Select Objective...";
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

        // Once the player is wearing a coat, his indicator will turn green
        // so that he is aware that he is wearing a coat.
        if (PointerController.IsWearingCoat == true)
        {
            _labCoatRenderer.material = _greenFlashIndicator;
        }
        else
        {
            _labCoatRenderer.material = _redFlashIndicator;
        }
		
        if (PointerController.IsWearingGlasses == true)
        {
            _labGlassesRenderer.material = _greenFlashIndicator;
        }
        else
        {
            _labGlassesRenderer.material = _redFlashIndicator;
        }
		
        if (PointerController.IsWearingGloves == true)
        {
            _labGlovesRenderer.material = _greenFlashIndicator;
        }
        else
        {
            _labGlovesRenderer.material = _redFlashIndicator;
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
                if (lookedAtButton.GetComponent<Image>() != null)
                {
                   // _image = lookedAtButton.GetComponent<Image>();
                    //_image.color = Color.red;
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                    Input.GetKeyDown(KeyCode.K))
                {
                    //_image.color = Color.green;
					
                    // Button functionality/behaviour on click events
                    if (lookedAtButton.name == "Use Water Bottle")
                    {
                        TextObject.GetComponent<Text>().text = string.Empty;
                        Instantiate(LabEquipment, GameObject.Find("Lab Desks").transform);
                        ProfileSystemController.TriesOnLevelMixColors++;
                        ProfileSystemController.PlayingALevel = true;
                        ProfileSystemController.UpdateProfileData();

                        //if (PointerController.IsWearingCoat == true)
                        //{
                        // Resetting the safety guidelines board's text current value
                        // so we can replace it with the new objectives'.
                        SafetyGuidelinesBoardText.text = "";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player wears a lab coat.\n";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player wears proper eye protection.\n";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player wears proper hand protection.\n";
                        SafetyGuidelinesBoardText.text += "* It is important that the player mixes the correct chemicals and disposes of any mistakes in their proper containers.\n";
                        SafetyGuidelinesBoardText.text += "* It is important that the player does not pour mixed solutions into their original containers.\n";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player washes any used glassware after they are done using them.\n";
                        
                        
                        SelectObjectiveUseWaterBottle();
							
                        CanOpenDoor = true;
							
                        // We dont want the player to think he should click any of these
                        // buttons before he actually can be able to.
                        DisableButtonsInteractivity();
							
                        // If the player is in fact wearing a coat, then flash the indicator as
                        // green once he takes the new objective.
                        //StartCoroutine(FlashIndicator(_labCoatIndicator, _greenFlashIndicator));
                        //}
                        //else
                        //{
                        // Otherwise flash a red indicator so that the player will immediately
                        // know if he needs to have equipment beforehand.
                        //StartCoroutine(FlashIndicator(_labCoatIndicator, _redFlashIndicator));
                        //}
                    } else if (lookedAtButton.name == "Use Teleporter")
                    {
                        TextObject.GetComponent<Text>().text = string.Empty;
                        Instantiate(LabEquipment, GameObject.Find("Lab Desks").transform);
                        SelectObjectiveUseTeleporter();
                        
                        CanOpenDoor = true;
                        
                        DisableButtonsInteractivity();
                    } else if (lookedAtButton.name == "Mix Colors")
                    {
                        Instantiate(LabEquipment, GameObject.Find("Lab Desks").transform);
                        ProfileSystemController.TriesOnLevelMixColors++;
                        ProfileSystemController.PlayingALevel = true;
                        ProfileSystemController.UpdateProfileData();

                        for (int i = 0; i < 4; i++)
                        {
                            TaskFields[i].SetActive(true);
                        }
                        
                        SafetyGuidelinesBoardText.text = "";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player wears a lab coat.\n";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player wears proper eye protection.\n";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player wears proper hand protection.\n";
                        SafetyGuidelinesBoardText.text += "* It is important that the player mixes the correct chemicals and disposes of any mistakes in their proper containers.\n";
                        SafetyGuidelinesBoardText.text += "* It is important that the player does not pour mixed solutions into their original containers.\n";
                        SafetyGuidelinesBoardText.text += "* It is mandatory that the player washes any used glassware after they are done using them.\n";
						
                        SelectObjectiveMixColors();
                        
                        CanOpenDoor = true;
                        
                        // We want to be able to go through the different pages of
                        // this objective because it contains more than 4 tasks.
                        _isNextPageAvailable = true;
                    } else if (lookedAtButton.name == "Next Page Button" && CurrentObjective == "Mix Colors" && _isNextPageAvailable)
                    {
                        GoForwardAPage();
                    } else if (lookedAtButton.name == "Prev Page Button" && CurrentObjective == "Mix Colors")
                    {
                        GoBackAPage();
                    } else if (lookedAtButton.name == "Volume Down")
                    {
                        _playerAudioSource.volume -= 0.05f;
                        // This plays the click sound once a button has been pressed
                        // so that the player can see how his sound setting was updated.
                        _playerAudioSource.PlayOneShot(_playerAudioSource.clip);
                    } else if (lookedAtButton.name == "Volume Up")
                    {
                        _playerAudioSource.volume += 0.05f;
                        _playerAudioSource.PlayOneShot(_playerAudioSource.clip);
                    }
                }
            }
            // If the player is holding an item and needs to return it, he must
            // be hovering over the original indicator for that item and press the
            // controller button to return the item and remove it from his hand.
        }
        else
        {
            /*
            if (_image != null)
            {
                _image.color = Color.white;
            }
            */
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

    public IEnumerator FlashIndicator(GameObject indicatorObject, Material indicatorFlash)
    {
        MeshRenderer indicatorMesh = indicatorObject.GetComponent<MeshRenderer>();
		
        for (int i = 0; i < 6; i++)
        {
            // Indicator flash is either the green or red indicator, depending
            // on the one we placed as a parameter when calling the coroutine.
            indicatorMesh.material = indicatorFlash;
            yield return new WaitForSeconds(0.4f);
            indicatorMesh.material = _normalIndicator;
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void SelectObjectiveUseWaterBottle()
    {
        CurrentObjective = "Use Water Bottle";
        PickedUpWaterBottle = false;
        PlacedBackWaterBottle = false;
        // Resetting the hand-in boolean once a new objective starts.
        HandinController.IsObjectiveHandedIn = false;
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
        HandinController.IsObjectiveHandedIn = false;
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
        HandinController.IsObjectiveHandedIn = false;
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
        CenterEyePointer.IsTeleporterReset = false;
        ResetEquipment();
	    
        for (int j = 0; j < TaskFields.Length; j++)
        {
            TaskFields[j].GetComponentInChildren<Image>().color = Color.white;
            TaskFields[j].GetComponentInChildren<Text>().text = "";
        }
		
        CurrentObjectiveTitle.GetComponent<Text>().text = "Objective: " + CurrentObjective;
        Title.GetComponent<Text>().text = "Currently: " + CurrentObjective;
        // This changes the objective text title on top of the door to the new objective.
        CurrentObjectiveText.GetComponent<Text>().text = CurrentObjective;
	    
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

    private void ResetEquipment()
    {
        // Now we have to return the equipment back to its place.
        CanWearEquipment = false;
		
        // Once the new objective in the lab has been selected, we
        // reset the player's safety equipment, so that he has to
        // put it on in the level in order to practice it.
        PointerController.IsWearingCoat = false;
        PointerController.IsWearingGlasses = false;
        PointerController.IsWearingGloves = false;

        GameObject labCoat = GameObject.Find("Lab Coat");
        labCoat.GetComponent<Rigidbody>().isKinematic = true;
        labCoat.GetComponent<MeshRenderer>().enabled = true;
        _player.GetComponentInChildren<PointerController>().ReturnEquipment();

        // We make sure that once the level is restarted, the lever will
        // return to its original position.
        GameObject lever = GameObject.FindGameObjectWithTag("Lever");
        LeverController leverScript = lever.GetComponent<LeverController>();
        leverScript.CanPushLever = true;
        if (leverScript.IsLeverDown)
        {
            leverScript.StartCoroutine(leverScript.PushLever(leverScript.IsLeverDown ? -leverScript.RotationMargin : leverScript.RotationMargin));
        }
    }
}