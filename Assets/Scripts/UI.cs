using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NetworkVelocityProvider VelocityProvider;
    public UIVM Uivm;
    public CartHandle Cart;
    private InputAction MenuAction;
    private UIDocument Document;

    private void Start()
    {
        VelocityProvider = Cart.gameObject.GetComponent<NetworkVelocityProvider>();
        MenuAction = InputSystem.actions.FindAction("Cancel");
        Document = GetComponent<UIDocument>();
        BindButtons();
    }

    private void BindButtons()
    {
        Document.rootVisualElement.Q<Button>("back").clicked += SwitchMenu;
        Document.rootVisualElement.Q<Button>("pit").clicked += () =>
        {
            Cart.TransferToPitRpc();
            SwitchMenu();
        };
    }

    void Update()
    {
        var velocity = VelocityProvider.Velocity.magnitude;
        Uivm.Speed = velocity;
        Uivm.UpdateSpeedKmh();
        Uivm.LapTime = $"{TimeSpan.FromSeconds(Time.timeAsDouble - Cart.LastLap.LapStart):g}";
        if (Cart.FastestLaps is not null)
        {
            try
            {
                var delta = Cart.LastLap.Delta(Cart.FastestLaps);
                var formated = $"{Math.Round(delta, 2):N2}";
                Uivm.Delta = formated;
            }
            catch
            {
            }
        }

        if (Cart.LastLap.IsValid)
        {
            Uivm.LapIndicator = Color.white;
        }
        else
        {
            Uivm.LapIndicator = Color.orangeRed;
        }

        Uivm.Lap = Cart.Laps.Count;

        if (MenuAction.WasPressedThisFrame())
        {
            SwitchMenu();
        }
    }

    private void SwitchMenu()
    {
        Uivm.ShowMenu = Uivm.ShowMenu switch
        {
            Visibility.Visible => Visibility.Hidden,
            Visibility.Hidden => Visibility.Visible
        };
    }
}