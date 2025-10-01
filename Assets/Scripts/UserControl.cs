using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UserControl : NetworkBehaviour
{
    public Guid Id = Guid.NewGuid();
    public Transform CameraPosition;

    public NetworkVariable<float> Angle = new(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);


    public NetworkVariable<float> Engine = new(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> Breaks = new(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public float CurrentAngle => Angle.Value;
    public float CurrentEngine => Engine.Value;

    public float CurrentBreaks => Breaks.Value;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>();

        if (IsClient && IsOwner)
        {
            var cam = GameObject.Find("Camera");
            cam.transform.parent = transform;
            cam.transform.localPosition = CameraPosition.localPosition;
            cam.transform.localRotation = CameraPosition.localRotation;

            var go = GameObject.Find("UI");
            go.GetComponent<UIDocument>().enabled = true;
            var ui = go.GetComponent<UI>();
            ui.enabled = true;
            ui.Cart = GetComponent<CartHandle>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Engine.Value != 0)
        {
            var a = 5;
        }
    }
}