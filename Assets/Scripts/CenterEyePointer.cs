using System.Collections;
using UnityEngine;

public class CenterEyePointer : MonoBehaviour
{
    public GameObject Player;
    public LayerMask PlayerLayerMask;
    public LayerMask DoorLayerMask;
    public GameObject PlayerDirection;
    public LayerMask TeleporterLayerMask;
    private GameObject _currentTeleporter;
    private bool _isAlphaIncreased = false;

    private Vector3 _newPlayerPosition;
    private Quaternion _newPlayerRotation;
    private Quaternion _newEyeRotation;
    private Transform _directionInTeleporter;

    public GameObject FadeTransitioner;
    // This is where the player will be teleporter to in the
    // world to start playing on the new objective's level
    public GameObject UseWaterBottleObjectiveTeleporter;
    private MeshRenderer _useWaterBottleTeleporterRenderer;
    //
    public GameObject MixColorsObjectiveTeleporter;
    private MeshRenderer _mixColorsTeleporterRenderer;

    public Material TeleporterAvailableMaterial;
    public Material TeleporterDisabledMaterial;
    public static bool IsTeleporterReset = false;
    public GameObject StartPlayerPosition;

    private void Start()
    {
        _useWaterBottleTeleporterRenderer = 
            UseWaterBottleObjectiveTeleporter.GetComponentInChildren<MeshRenderer>();
        
        _mixColorsTeleporterRenderer = 
            MixColorsObjectiveTeleporter.GetComponentInChildren<MeshRenderer>();
        
        var teleporters = GameObject.FindGameObjectsWithTag("Teleporter");
        FadeTransitioner.SetActive(false);

        foreach (var teleporter in teleporters)
        {
            var objectsInTelerpoter = teleporter.GetComponentsInChildren<Transform>();

            // We dont want the pointers for the angle of the teleporters to
            // be visible once the game starts. Theyre only for the designers to configure.
            foreach (var obj in objectsInTelerpoter)
            {
                if (obj.CompareTag("Direction"))
                {
                    obj.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
            }
        }
        
        var teleporterDirections = GameObject.FindGameObjectsWithTag("DoorDirection");
        
        foreach (var teleporter in teleporterDirections)
        {
            var objectsInTelerpoter = teleporter.GetComponentsInChildren<Transform>();

            // We dont want the pointers for the angle of the teleporters to
            // be visible once the game starts. Theyre only for the designers to configure.
            foreach (var obj in objectsInTelerpoter)
            {
                if (obj != null) {
                    obj.GetComponentInChildren<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    private void StartGamePosition()
    {
        Invoke("FadeOutTeleporter", 0.3f);
        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", false);

        Vector3 newPlayerPosition = Player.transform.position;
        newPlayerPosition = StartPlayerPosition.transform.position;
        Player.transform.position = newPlayerPosition;
    }

    void Update () {
        if (ProfileSystemController._isGameStarted)
        {
            FadeTransitioner.SetActive(true);
            FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", true);
            Invoke("StartGamePosition", 0.3f);

            ProfileSystemController._isGameStarted = false;
        }

        if (HandinController.IsObjectiveHandedIn && IsTeleporterReset == false)
        {
            _useWaterBottleTeleporterRenderer.material = TeleporterAvailableMaterial;
            _mixColorsTeleporterRenderer.material = TeleporterAvailableMaterial;
            IsTeleporterReset = true;
            UseWaterBottleObjectiveTeleporter.GetComponent<BoxCollider>().enabled = true;
            MixColorsObjectiveTeleporter.GetComponent<BoxCollider>().enabled = true;
        }
        else if (HandinController.IsObjectiveHandedIn == false &&
                 ObjectivesSelector.CurrentObjective == "Use Water Bottle" ||
                 ObjectivesSelector.CurrentObjective == "Mix Colors")
        {
            _useWaterBottleTeleporterRenderer.material = TeleporterDisabledMaterial;
            UseWaterBottleObjectiveTeleporter.GetComponent<BoxCollider>().enabled = false;
            
            _mixColorsTeleporterRenderer.material = TeleporterDisabledMaterial;
            MixColorsObjectiveTeleporter.GetComponent<BoxCollider>().enabled = false;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        Ray rayDoor = new Ray(transform.position, transform.forward);
        RaycastHit hitDoor;
                
        _newPlayerPosition = Player.transform.position;
        _newPlayerRotation = Player.transform.rotation;
        _newEyeRotation = PlayerDirection.transform.rotation;

        // If the player is allowed to open the door and click on it while
        // looking at it, then we transport him to the new objective level.
        if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
             Input.GetKeyDown(KeyCode.T)) && ObjectivesSelector.CanOpenDoor)
        {
            if (Physics.Raycast(rayDoor, out hitDoor, 100, DoorLayerMask))
            {
                if (ObjectivesSelector.CurrentObjective == "Use Water Bottle")
                {
                    StartCoroutine(TeleportPlayerTo(UseWaterBottleObjectiveTeleporter));
                } else if (ObjectivesSelector.CurrentObjective == "Mix Colors")
                {
                    StartCoroutine(TeleportPlayerTo(MixColorsObjectiveTeleporter));
                }
            } 
        }
		
        // Checking if the player is hovering over an item with the controller
        if (Physics.Raycast(ray, out hit, 100, TeleporterLayerMask))
        {
            // We dont put it in the function because the else statement
            // would have no access to a variable that wont exist due to
            // the ray not hitting anymore and not assigning it in the first place!
            _currentTeleporter = hit.collider.gameObject;
            if (_isAlphaIncreased == false)
            {
                UpdateTeleporterAlpha(hit, 0.4f);
                _isAlphaIncreased = true;
            }
			
            // If we use a teleporter while we are still working on completing the
            // objective, then we use the stardard teleporter mechanic instead of
            // going to the lobby door.
            if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                 Input.GetKeyDown(KeyCode.T)) && _isAlphaIncreased && HandinController.IsObjectiveHandedIn &&
                (_currentTeleporter.name == "Teleporter: Use Water Bottle" || 
                _currentTeleporter.name == "Teleporter: Mix Colors")) // Or another objective Or and so on...
            {
                ObjectivesSelector.UsedTeleporter = true;

                var doorDirection = GameObject.FindGameObjectWithTag("DoorDirection");

                _directionInTeleporter = doorDirection.transform;
                FadeTransitioner.SetActive(true);
                FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", true);
                Invoke("UpdatePlayerPosition", 0.3f);
            }
            // This teleports the player to the teleporter's position.
            // The second input is specifically for testing for the PC instead of
            // wearing the oculus itself.
            else if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
                      Input.GetKeyDown(KeyCode.T)) && _isAlphaIncreased && HandinController.IsObjectiveHandedIn == false)
            {
                ObjectivesSelector.UsedTeleporter = true;
				
                var objectsInTelerpoter = _currentTeleporter.GetComponentsInChildren<Transform>();

                // This makes sure we target that child in the teleporter object
                // that contains the angle we want for the player to be transported with.
                foreach (var obj in objectsInTelerpoter)
                {
                    if (obj.CompareTag("Direction"))
                    {
                        _directionInTeleporter = obj;
                        FadeTransitioner.SetActive(true);
                        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", true);
                        Invoke("UpdatePlayerPosition", 0.3f);
                    }
                }
            }
        }
        else
        {
            if (_isAlphaIncreased)
            {
                UpdateTeleporterAlpha(hit, -0.4f);
                _isAlphaIncreased = false;
            }
        }
    }

    // This function makes sure we update the teleporter's visibility
    // without having to rewrite the same code twice when the player vision
    // enters and exits the teleporter
    private void UpdateTeleporterAlpha(RaycastHit hit, float newAlphaValue)
    {
        var childRenderers = _currentTeleporter.GetComponentsInChildren<MeshRenderer>();
        foreach (var childRenderer in childRenderers)
        {
            var childRendererColor = childRenderer.material.color;
            childRendererColor.a += newAlphaValue;
            childRenderer.material.color = childRendererColor;
        }
    }

    private void UpdatePlayerPosition()
    {
        Invoke("FadeOutTeleporter", 0.3f);
        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", false);
        _newPlayerPosition = _directionInTeleporter.transform.position;
        _newPlayerRotation = _directionInTeleporter.transform.rotation;
        _newEyeRotation = _directionInTeleporter.transform.rotation;

        // This makes sure he doesnt teleport in the ground itself
        // and turn him using the child object of the teleporter's angle of rotation
        _newPlayerPosition.y += 1.3f;
        Player.transform.position = _newPlayerPosition;
        Player.transform.rotation = _newPlayerRotation;
        PlayerDirection.transform.rotation = _newEyeRotation;
        // I'm rotating the pose too because it stays the same
        // once the player teleports to a new destination.
        OVRManager.display.RecenterPose();
        //FadeTransitioner.SetActive(false);
    }

    private void FadeOutTeleporter()
    {
        FadeTransitioner.SetActive(false);   
    }

    public IEnumerator TeleportPlayerTo(GameObject levelTeleporter)
    {
        FadeTransitioner.SetActive(true);
        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", true);
        
        Transform directionInTeleporter = null;
        var objectsInTelerpoter = levelTeleporter.GetComponentsInChildren<Transform>();

        // This makes sure we target that child in the teleporter object
        // that contains the angle we want for the player to be transported with.
        foreach (var obj in objectsInTelerpoter)
        {
            if (obj.CompareTag("Direction"))
            {
                directionInTeleporter = obj;
            }
        }
        
        yield return new WaitForSeconds(0.3f);
		
        Invoke("FadeOutTeleporter", 0.3f);
        FadeTransitioner.GetComponent<Animator>().SetBool("toggleTransitioner", false);
		
        Vector3 newPlayerPos = Player.transform.position;
        Quaternion newPlayerRot = Player.transform.rotation;
        Quaternion newEyesRot = PlayerDirection.transform.rotation;
		
        newPlayerPos = directionInTeleporter.transform.position;
        newPlayerRot = directionInTeleporter.transform.rotation;
        newEyesRot = directionInTeleporter.transform.rotation;

        // This makes sure he doesnt teleport in the ground itself
        // and turn him using the child object of the teleporter's angle of rotation
        newPlayerPos.y += 1.3f;
        Player.transform.position = newPlayerPos;
        Player.transform.rotation = newPlayerRot;
        PlayerDirection.transform.rotation = newEyesRot;
        
        OVRManager.display.RecenterPose();
    }
}