﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandinController : MonoBehaviour
{
    private string _objectiveReport = "";
    public GameObject TextObject;
	
    public GameObject Pointer;
    public LayerMask CatchButtonLayer;
    //private Image _image;

    public static bool IsObjectiveHandedIn = false;

    private GameObject _lookedAtButton;
    private GameObject _player;
    private GameObject _lobbyPoint;

    public GameObject FadeTransitioner;
    private GameObject _objectivesSelector;

    public static bool _enableTransitioner = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _lobbyPoint = GameObject.FindGameObjectWithTag("Restart Position");
        _objectivesSelector = GameObject.Find("Tablet For Selecting Objectives");

        FadeTransitioner.SetActive(false);
    }

    private void Update()
    {
        if (_enableTransitioner)
        {
            _player.GetComponent<CharacterController>().enabled = false;
            FadeTransitioner.SetActive(true);
            FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", true);
            Invoke("FadeOutTransitioner", 0.3f);

            _enableTransitioner = false;
        }

        Ray ray = new Ray(Pointer.transform.position, Pointer.transform.forward);
        RaycastHit hit;
		
        Debug.DrawRay(Pointer.transform.position, Pointer.transform.forward);
		
        // If the player happens to hover over UI elements such as
        // buttons, then they will be updated here on different events.
        if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
        {
            _lookedAtButton = hit.collider.gameObject;
            if (_lookedAtButton.GetComponent<Image>() != null)
            {
               // _image = lookedAtButton.GetComponent<Image>();
                //_image.color = Color.red;
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                Input.GetKeyDown(KeyCode.K))
            {
                //_image.color = Color.green;
					
                // Button functionality/behaviour on click events
                if (_lookedAtButton.name == "Hand-In Button" && _lookedAtButton.GetComponent<Button>().interactable == true)
                {
                    _lookedAtButton.GetComponent<Button>().interactable = false;
                    Invoke("MakeButtonInteractable", 4f);
                    HandInObjective();
                }
            }
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
    }

    private void MakeButtonInteractable()
    {
        _lookedAtButton.GetComponent<Button>().interactable = true;
    }

    public void HandInObjective()
    {
        // Resetting the objective report string before we append text to it!
        _objectiveReport = "";
        // We are checking for every task in the objective and depending
        // on its outcome, we update the final mission report string and then
        // set it as the text parameter in the canvas text object.
        _objectiveReport += "Objective report:\n";

        GameObject[] existingDirtyBeakers = GameObject.FindGameObjectsWithTag("Dirty Beaker");
        GameObject[] existingSmellyWaste = GameObject.FindGameObjectsWithTag("Smelly Waste");

        if (ObjectivesSelector.CurrentObjective == "Use Water Bottle")
        {
            if (ObjectivesSelector.PickedUpWaterBottle == false)
            {
                _objectiveReport += "Did not pick up a water bottle!\n";
            }
            else
            {
                _objectiveReport += "DONE: Pick up a water bottle!\n";
            }

            if (ObjectivesSelector.PlacedBackWaterBottle == false)
            {
                _objectiveReport += "Did not place back a water bottle!\n";
            }
            else
            {
                _objectiveReport += "DONE: Place back a water bottle!\n";
            }

            if (PointerController.IsWearingCoat == false)
            {
                _objectiveReport += "You are not wearing a lab coat!\n";
            }
            else
            {
                _objectiveReport += "DONE: Wore a lab coat\n";
            }

            if (existingDirtyBeakers.Length > 0)
            {
                _objectiveReport += "You've left dirty beakers in the lab!\n";
            }
            else
            {
                _objectiveReport += "You've cleaned the beakers in the lab!\n";
            }

            if (existingSmellyWaste.Length > 0)
            {
                _objectiveReport += "You've left smelly waste in the lab!\n";
            } else
            {
                _objectiveReport += "You've cleaned the smelly waste in the lab!\n";
            }
            

            // Calculating the pass or fail of an objective and giving the result of it.
            if (ObjectivesSelector.PickedUpWaterBottle &&
                ObjectivesSelector.PlacedBackWaterBottle &&
                PointerController.IsWearingCoat &&
                existingDirtyBeakers.Length <= 0 &&
                existingSmellyWaste.Length <= 0)
            {
                ProfileSystemController.TimesForLevelMixColors.Add(ProfileSystemController.CurrentLevelSeconds);
                //Debug.Log(ProfileSystemController.CurrentLevelSeconds);
                ProfileSystemController.CurrentLevelSeconds = 0;

                _objectiveReport = "Objective complete!\n";
                _objectiveReport += "Returning to lobby in 3...";
                Invoke("CompleteHandIn", 4f);
            }
            else
            {
                ProfileSystemController.TimesAGuidelineIsMissed++;
                ProfileSystemController.UpdateProfileData();

                _objectiveReport += "< Failed >";
            }

            TextObject.GetComponent<Text>().text = _objectiveReport;
        }
		
        if (ObjectivesSelector.CurrentObjective == "Use Teleporter")
        {
            if (ObjectivesSelector.UsedTeleporter == false)
            {
                _objectiveReport += "Did not use a teleporter!\n";
            }

            if (existingDirtyBeakers.Length > 0)
            {
                _objectiveReport += "You've left dirty beakers in the lab!\n";
            }
            else
            {
                _objectiveReport += "You've cleaned the beakers in the lab!\n";
            }

            if (existingDirtyBeakers.Length > 0)
            {
                _objectiveReport += "You've left dirty beakers in the lab!\n";
            }
            else
            {
                _objectiveReport += "You've cleaned the smelly waste in the lab!\n";
            }

            // Calculating the pass or fail of an objective and giving the result of it.
            if (ObjectivesSelector.UsedTeleporter &&
                existingDirtyBeakers.Length <= 0 &&
                existingSmellyWaste.Length <= 0)
            {
                Invoke("CompleteHandIn", 4f);
            }
            else
            {
                _objectiveReport += "< Failed >";
            }

            TextObject.GetComponent<Text>().text = _objectiveReport;
        }
        
        if (ObjectivesSelector.CurrentObjective == "Mix Colors")
        {
            if (ObjectivesSelector.PourRedIntoTube == false) { _objectiveReport += "Did not pour red into tube!\n"; }
            if (ObjectivesSelector.PourBlueIntoTube == false) { _objectiveReport += "Did not pour blue into tube!!\n"; }
            if (ObjectivesSelector.PourYellowIntoTube == false) { _objectiveReport += "Did not pour yellow into tube!\n"; }
            if (ObjectivesSelector.MixRedAndYellow == false) { _objectiveReport += "Did not mix red and yellow!\n"; }
            if (ObjectivesSelector.MixRedAndBlue == false) { _objectiveReport += "Did not mix red and blue!\n"; }
            if (ObjectivesSelector.MixBlueAndYellow == false) { _objectiveReport += "Did not mix blue and yellow!\n";}

            if (existingDirtyBeakers.Length > 0)
            {
                _objectiveReport += "You've left dirty beakers in the lab!\n";
            }
            else
            {
                _objectiveReport += "You've cleaned the beakers in the lab!\n";
            }

            if (existingDirtyBeakers.Length > 0)
            {
                _objectiveReport += "You've left dirty beakers in the lab!\n";
            }
            else
            {
                _objectiveReport += "You've cleaned the smelly waste in the lab!\n";
            }

            // Calculating the pass or fail of an objective and giving the result of it.
            if (ObjectivesSelector.PourRedIntoTube &&
                ObjectivesSelector.PourBlueIntoTube &&
                ObjectivesSelector.PourYellowIntoTube &&
                ObjectivesSelector.MixBlueAndYellow &&
                ObjectivesSelector.MixRedAndBlue &&
                ObjectivesSelector.MixRedAndYellow &&
                existingDirtyBeakers.Length <= 0 &&
                existingSmellyWaste.Length <= 0)
            {
                Invoke("CompleteHandIn", 4f);
            }
            else
            {
                ProfileSystemController.TimesAGuidelineIsMissed++;
                ProfileSystemController.UpdateProfileData();
                TextObject.GetComponent<Text>().text = string.Empty;

                _objectiveReport += "< Failed >";
            }

            TextObject.GetComponent<Text>().text = _objectiveReport;
        }
    }

    private void CompleteHandIn()
    {
        ObjectivesSelector.IsNotificationBarFlashed = false;
        _objectivesSelector.GetComponent<ObjectivesSelector>().NotificationBar.SetActive(false);

        _player.GetComponent<SamplePlayerController>().EnableLinearMovement = false;
        _player.GetComponent<CharacterController>().enabled = false;
        FadeTransitioner.SetActive(true);
        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", true);
        Invoke("FadeOutTransitioner", 0.3f);

        ProfileSystemController.PlayingALevel = false;

        _objectiveReport += "< Success >";
        
        GameObject labEquipment = GameObject.FindGameObjectWithTag("Lab Equipment");

        // The objective has to be only successfully handed in before
        // you are allowed to pick a new one!
        IsObjectiveHandedIn = true;

        ObjectivesSelector.CanWearEquipment = false;
        PointerController.IsWearingCoat = false;
        PointerController.IsWearingGlasses = false;
        PointerController.IsWearingGloves = false;


        PointerController.IsHoldingItem = false;
        PointerController.CurrentlyHoldingObjectForBeakers = null;

        _player.GetComponentInChildren<PointerController>().ReturnEquipment();
        Destroy(labEquipment);
    }

    public void FadeOutTransitioner()
    {
        Invoke("FadeOutTeleporter", 0.3f);
        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", false);

        Vector3 newPlayerPos = _player.transform.position;
        newPlayerPos = _lobbyPoint.transform.position;
        _player.transform.position = newPlayerPos;

        Quaternion _newPlayerRotation = _lobbyPoint.transform.rotation;
        _player.transform.rotation = _newPlayerRotation;
    }

    public void FadeOutTeleporter()
    {
        FadeTransitioner.SetActive(false);
    }
}