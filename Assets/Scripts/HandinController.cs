using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandinController : MonoBehaviour
{
    private string _objectiveReport = "";
    public GameObject TextObject;
	
    public GameObject Pointer;
    public LayerMask CatchButtonLayer;
    private Image _image;

    public static bool _isObjectiveHandedIn = false;
    
    private void Update()
    {
        Ray ray = new Ray(Pointer.transform.position, Pointer.transform.forward);
        RaycastHit hit;
		
        Debug.DrawRay(Pointer.transform.position, Pointer.transform.forward);
		
        // If the player happens to hover over UI elements such as
        // buttons, then they will be updated here on different events.
        if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
        {
            var lookedAtButton = hit.collider.gameObject;
            _image = lookedAtButton.GetComponent<Image>();
            _image.color = Color.red;

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                Input.GetKeyDown(KeyCode.K))
            {
                _image.color = Color.green;
					
                // Button functionality/behaviour on click events
                if (lookedAtButton.name == "Hand-In Button")
                {
                    HandInObjective();
                }
            }
        }
        else
        {
            if (_image != null)
            {
                _image.color = Color.white;
            }
        }
    }

    public void HandInObjective()
    {
        // Resetting the objective report string before we append text to it!
        _objectiveReport = "";
        // We are checking for every task in the objective and depending
        // on its outcome, we update the final mission report string and then
        // set it as the text parameter in the canvas text object.
        _objectiveReport += "Objective report:\n";
		
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

            // Calculating the pass or fail of an objective and giving the result of it.
            if (ObjectivesSelector.PickedUpWaterBottle &&
                ObjectivesSelector.PlacedBackWaterBottle)
            {
                _objectiveReport += "< Success >";
                // The objective has to be only successfully handed in before
                // you are allowed to pick a new one!
                _isObjectiveHandedIn = true;
            }
            else
            {
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

            // Calculating the pass or fail of an objective and giving the result of it.
            if (ObjectivesSelector.UsedTeleporter)
            {
                _objectiveReport += "< Success >";
                _isObjectiveHandedIn = true;
            }
            else
            {
                _objectiveReport += "< Failed >";
            }

            TextObject.GetComponent<Text>().text = _objectiveReport;
        }
    }
}