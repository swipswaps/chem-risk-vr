using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetWaterController : MonoBehaviour
{
    private GameObject _labEquipment;
    public GameObject GreenYellowParticles;
    public GameObject SmokeParticles;
    private bool _areParticlesInitiated = false;
    private bool _isSmokeInitiated = false;
    private GameObject _currentSmokeParticles;

    private GameObject _player;
    private GameObject _lobbyPoint;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _lobbyPoint = GameObject.FindGameObjectWithTag("Restart Position");
        _labEquipment = GameObject.FindGameObjectWithTag("Lab Equipment");
    }

    private void OnTriggerExit(Collider other)
    {
        // the smoke will stop generating after the players leaves the beaker
        // away from the sink and stops the sink from running water.
        _isSmokeInitiated = false;
        Invoke("RemoveParticles", 2f);
    }

    private void OnTriggerEnter(Collider other)
	{
        if (_areParticlesInitiated == false &&
            (other.gameObject.name == "Red Substance Beaker(Clone)" || other.gameObject.name == "Orange Substance Beaker(Clone)" || other.gameObject.name == "Yellow Substance Beaker(Clone)"))
        {
            Invoke("GameOver", 3f);
            GameObject deadlyParticles = Instantiate(GreenYellowParticles, gameObject.transform);
            deadlyParticles.transform.Rotate(180, 0, 0);
            _areParticlesInitiated = true;
        }

        if (_isSmokeInitiated == false &&
            (other.gameObject.name == "Blue Substance Beaker(Clone)" || other.gameObject.name == "Purple Substance Beaker(Clone)"))
        {
            _currentSmokeParticles = Instantiate(SmokeParticles, gameObject.transform);
            _currentSmokeParticles.transform.Rotate(180, 0, 0);
            _isSmokeInitiated = true;
        }

        if (other.CompareTag("Beaker"))
		{
			Destroy(other.transform.GetChild(0).gameObject);

            if (other.transform.gameObject.transform.GetChild(0).name != "Smelly Waste(Clone)")
            {
                ProfileSystemController.TimesAnIncidentWasCaused++;
                ProfileSystemController.UpdateProfileData();
            }

			if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratorium_flask_T2 Instance")
            {
                other.name = "Round Empty Beaker";
            } else if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratorium_flask Instance")
            {
                other.name = "Empty Beaker";
            }
            else if (other.GetComponent<MeshFilter>().mesh.name == "mod_laboratory_beaker Instance")
            {
                other.name = "Big Empty Beaker";
            }
            else if (other.GetComponent<MeshFilter>().mesh.name == "laboratory_testTube_tube Instance")
            {
                other.name = "Small Empty Beaker";
            }
            gameObject.tag = "Untagged";
		}
		if (other.CompareTag("Dropper"))
		{
			Destroy(other.transform.GetChild(0).gameObject);
			other.name = "Empty Dropper";
		}
	}

    private void RemoveParticles()
    {
        Destroy(_currentSmokeParticles);
    }

    private void GameOver()
    {
        Vector3 newPlayerPos = _player.transform.position;
        newPlayerPos = _lobbyPoint.transform.position;
        _player.transform.position = newPlayerPos;

        HandinController.IsObjectiveHandedIn = true;
        ProfileSystemController.TimesAGuidelineIsMissed++;
        ProfileSystemController.PlayingALevel = false;
        ProfileSystemController.UpdateProfileData();

        ObjectivesSelector.CanWearEquipment = false;
        PointerController.IsWearingCoat = false;
        PointerController.IsWearingGlasses = false;
        PointerController.IsWearingGloves = false;

        PointerController.IsHoldingItem = false;
        PointerController.CurrentlyHoldingObjectForBeakers = null;
        _player.GetComponentInChildren<PointerController>().ReturnEquipment();
        Destroy(_labEquipment);
    }
}
