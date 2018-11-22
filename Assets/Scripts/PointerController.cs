using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
	//public GameObject Timer;
	private GameObject _lookedAtObject = null;
	public Material DefaultObjectMaterial;
	public Material DefaultItemMaterial;
	public Material HoverableObjectMaterial;
	private MeshRenderer _meshRenderer;
	private Image _image;
	public LayerMask CatchItemLayer;
	public LayerMask CatchButtonLayer;

	private bool _isHoldingItem = false;

	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		
		// Checking if the player is hovering over an item with the controller
		if (Physics.Raycast(ray, out hit, 100, CatchItemLayer))
		{
			// Only display the hovering effect IF the player is not
			// holding an item in his hand
			if (_isHoldingItem == false)
			{
				_lookedAtObject = hit.collider.gameObject;
				_meshRenderer = _lookedAtObject.GetComponent<MeshRenderer>();
				_meshRenderer.material = HoverableObjectMaterial;
			}

			if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && _isHoldingItem == false)
			{
				// We are positioning the item being held at the same spot as
				// the controller and then we are also rotating it based on how
				// the player is rotating the controller.
				var newItemPosition = _lookedAtObject.transform.position;
				newItemPosition = transform.position;
				_lookedAtObject.transform.position = newItemPosition;
				
				var newItemRotation = _lookedAtObject.transform.rotation;
				newItemRotation = transform.rotation;
				_lookedAtObject.transform.rotation = newItemRotation;

				_isHoldingItem = true;
			}
		}
		// Checking if the player is hovering over a button with the controller
		else if (Physics.Raycast(ray, out hit, 100, CatchButtonLayer))
		{
			if (_isHoldingItem == false)
			{
				_lookedAtObject = hit.collider.gameObject;
				_image = _lookedAtObject.GetComponent<Image>();
				_image.color = Color.red;
			}

			if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
			{
				_image.color = Color.green;
			}
		}
		else
		{
			if (_image != null)
			{
				_image.color = Color.white;
			}
			if (_meshRenderer != null)
			{
				_meshRenderer.material = DefaultItemMaterial;
			}
		}

		if (_isHoldingItem)
		{
			var newItemPosition = _lookedAtObject.transform.position;
			newItemPosition = transform.position;
			_lookedAtObject.transform.position = newItemPosition;
				
			var newItemRotation = _lookedAtObject.transform.rotation;
			newItemRotation = transform.rotation;
			_lookedAtObject.transform.rotation = newItemRotation;
		}
	}
}
