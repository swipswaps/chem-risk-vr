using UnityEngine;

public class CenterEyePointer : MonoBehaviour
{
	public GameObject Player;
	public LayerMask PlayerLayerMask;
	public LayerMask TeleporterLayerMask;
	private GameObject _currentTeleporter;
	private bool _isAlphaIncreased = false;

	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		
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
			
			// This teleports the player to the teleporter's.
			// The second input is specifically for testing for the PC instead of
			// wearing the oculus itself.
			if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) ||
			    _isAlphaIncreased && Input.GetKeyDown(KeyCode.T))
			{
				var newPlayerPos = Player.transform.position;
				newPlayerPos = _currentTeleporter.transform.position;
				// This makes sure he doesnt teleport in the ground itself
				newPlayerPos.y += 1.3f;
				Player.transform.position = newPlayerPos;
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
}
