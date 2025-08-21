using UnityEngine;
using UnityEngine.InputSystem;

public class ForceTest : MonoBehaviour
{
    private InputAction _forceAction;
    private Rigidbody _rigidbody;
    public Transform ForceTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _forceAction = InputSystem.actions.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var movement = _forceAction.ReadValue<Vector2>();
        if ( movement.y != 0)
        {
            Debug.Log($"Adding force: {movement}");
            _rigidbody.AddForce(transform.forward * (movement.y * 1000));
        }
    }
}
