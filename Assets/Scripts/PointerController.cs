using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    //public GameObject Timer;
    private GameObject _lookedAtObject = null;
    private GameObject _currentlyHoldingObject = null;
    public Material DefaultReturnItemMaterial;
    public Material DefaultItemMaterial;
    public Material HoverableObjectMaterial;
    private MeshRenderer _meshRenderer;
    private Image _image;
    public LayerMask CatchItemLayer;
    public LayerMask CatchButtonLayer;
    public LayerMask ReturnObjectLayer;

    private Transform _tempPosition = null;
    private bool _canReturnObject = false;

    private bool _isHoldingItem = false;

    // Update is called once per frame
    void Update () {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // We are looking with the controllerfor an object
        // that is of layer item
        if (Physics.Raycast(ray, out hit, 100, CatchItemLayer))
        {
            _lookedAtObject = hit.collider.gameObject;
            var parentOfTemporaryObject = _lookedAtObject.transform.parent;
            
            // Once it is found we change it to look as if it is hovered
            // so that it appears interact-able
            _meshRenderer = _lookedAtObject.GetComponent<MeshRenderer>();
            _meshRenderer.material = HoverableObjectMaterial;

            // If the player is not holding an interactable item and presses
            // the controller button, we append the item onto his hand and
            // make the one he clicked on become transparent, to indicate where
            // it must be returned after it is done being used.
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.K) && _isHoldingItem == false)
            {
                _currentlyHoldingObject = Instantiate(_lookedAtObject, parentOfTemporaryObject.transform);
                _currentlyHoldingObject.GetComponent<MeshRenderer>().material = DefaultItemMaterial;
                _currentlyHoldingObject.GetComponent<CapsuleCollider>().enabled = false;
                
                _meshRenderer.material = DefaultReturnItemMaterial;
                _lookedAtObject.layer = 21;
                _lookedAtObject.tag = "Return Item";
                
                if (_lookedAtObject.name == "Water") { ObjectivesController.PickedUpWaterBottle = true; }
                // This gives a small delay before the player can return
                // the object back to its place, because for some reason oculus
                // had a glitch and returned it immediately and it was very buggy.
                Invoke("EnableReturningObject", 2f);
                _isHoldingItem = true;
            }
            // If the player happens to hover over UI elements such as
            // buttons, then they will be updated here on different events.
        } else if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
        {
            _lookedAtObject = hit.collider.gameObject;
            _image = _lookedAtObject.GetComponent<Image>();
            _image.color = Color.red;

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                _image.color = Color.green;
            }
            // If the player is holding an item and needs to return it, he must
            // be hovering over the original indicator for that item and press the
            // controller button to return the item and remove it from his hand.
        } else if (Physics.Raycast(ray, out hit, 100, ReturnObjectLayer) && _canReturnObject)
        {
            _lookedAtObject = hit.collider.gameObject;
            _meshRenderer = _lookedAtObject.GetComponent<MeshRenderer>();
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.K))
            {
                _lookedAtObject.tag = "Untagged";
                _lookedAtObject.layer = 18;
                _meshRenderer.material = DefaultItemMaterial;

                _isHoldingItem = false;

                if (_lookedAtObject.name == "Water") { ObjectivesController.PlacedBackWaterBottle = true; }
                Destroy(_currentlyHoldingObject);
                _canReturnObject = false;
            }
        }
        else
        {
            if (_image != null)
            {
                _image.color = Color.white;
            }
            if (_meshRenderer != null && _isHoldingItem == false)
            {
                _meshRenderer.material = DefaultItemMaterial;
            }
        }

        // If an item is to be returned, then it will be tracked and
        // highlighted in the world, because the rays were having trouble
        // updating its material.
        var itemToReturn = GameObject.FindGameObjectWithTag("Return Item");
        if (itemToReturn != null)
        {
            itemToReturn.GetComponent<MeshRenderer>().material = DefaultReturnItemMaterial;
        }
        
        // This makes sure the item the player is holding is being placed
        // at the spot where the controller hand is.
        if (_isHoldingItem && _currentlyHoldingObject != null)
        {
            var newItemPosition = _currentlyHoldingObject.transform.position;
            newItemPosition = transform.position;
            _currentlyHoldingObject.transform.position = newItemPosition;
				
            var newItemRotation = _currentlyHoldingObject.transform.rotation;
            newItemRotation = transform.rotation;
            _currentlyHoldingObject.transform.rotation = newItemRotation;
        }
    }

    private void EnableReturningObject()
    {
        _canReturnObject = true;
    }
}