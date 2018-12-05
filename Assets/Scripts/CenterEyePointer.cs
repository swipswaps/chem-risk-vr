using UnityEngine;

public class CenterEyePointer : MonoBehaviour
{
    public GameObject Player;
    public LayerMask PlayerLayerMask;
    public GameObject PlayerDirection;
    public LayerMask TeleporterLayerMask;
    private GameObject _currentTeleporter;
    private bool _isAlphaIncreased = false;

    private Vector3 _newPlayerPosition;
    private Quaternion _newPlayerRotation;
    private Quaternion _newEyeRotation;
    private Transform _directionInTeleporter;

    public GameObject FadeTransitioner;

    private void Start()
    {
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
    }

    // Update is called once per frame
    void Update () {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
                
        _newPlayerPosition = Player.transform.position;
        _newPlayerRotation = Player.transform.rotation;
        _newEyeRotation = PlayerDirection.transform.rotation;
		
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
			
            // This teleports the player to the teleporter's position.
            // The second input is specifically for testing for the PC instead of
            // wearing the oculus itself.
            if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) ||
                _isAlphaIncreased && Input.GetKeyDown(KeyCode.T))
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
}