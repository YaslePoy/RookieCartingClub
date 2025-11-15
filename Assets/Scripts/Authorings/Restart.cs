using UnityEngine;
using UnityEngine.InputSystem;

public class Restart : MonoBehaviour
{
    private InputAction _resetAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _resetAction = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    private void Update()
    {
        if (_resetAction.WasPressedThisFrame()) GameObject.FindWithTag("Cart").GetComponent<TrackPlacement>().Start();
    }
}