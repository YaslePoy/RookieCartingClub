using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public static UI Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IVelocityProvider VelocityProvider;
    public UIVM Uivm;
    public CartHandle Cart;
    private InputAction MenuAction;
    private UIDocument Document;
    public bool InPitRequest;
    public void Start()
    {
        Instance = this;
        Uivm.ShowMenu = Visibility.Hidden;
        MenuAction = InputSystem.actions.FindAction("Cancel");
        Document = GetComponent<UIDocument>();
        BindButtons();
        // VelocityProvider = Cart.gameObject.GetComponent<NetworkVelocityProvider>();
    }

    public void BindButtons()
    {
        Document.rootVisualElement.Q<Button>("back").clicked += SwitchMenu;
        Document.rootVisualElement.Q<Button>("pit").clicked += () =>
        {
            // Cart.TransferToPitRpc();
            InPitRequest = true;
            SwitchMenu();
        };
        Document.rootVisualElement.Q<Button>("quit").clicked += () =>
        {
            // NetworkManager.Singleton?.Shutdown();
            Destroy(GameObject.Find("Network"));
            SceneManager.LoadScene("SelectorScene");
        };
    }

    public void UpdateUI()
    {
        if (RaceControl.Singleton.racers.Count != 0)
        {
            Cart = RaceControl.Singleton.racers[0];
        }
        
        var velocity = VelocityProvider.Velocity.magnitude;
        Uivm.Speed = velocity;
        Uivm.UpdateSpeedKmh();
        
        if (MenuAction.WasPressedThisFrame())
        {
            SwitchMenu();
        }
        // return;
        Uivm.LapTime = $"{TimeSpan.FromSeconds(Time.timeAsDouble - Cart.CurrentLap.LapStart):g}";
        var fastestTime = 0.0;
        if (Cart.FastestLaps is not null)
        {
            fastestTime = Cart.FastestLaps.TotalLapTime;
        }

        Uivm.FastestLapTime = $"{TimeSpan.FromSeconds(fastestTime):g}";
        var lastTime = 0.0;
        if (Cart.Laps.Count > 1)
        {
            lastTime = Cart.Laps[^2].TotalLapTime;
        }

        Uivm.LastLapTime = $"{TimeSpan.FromSeconds(lastTime):g}";
        if (Cart.FastestLaps is not null)
        {
            try
            {
                var delta = Cart.CurrentLap.Delta(Cart.FastestLaps);
                var formated = $"{Math.Round(delta, 2):N2}";
                Uivm.Delta = formated;
            }
            catch
            {
            }
        }


        if (Cart.CurrentLap.IsValid)
        {
            Uivm.LapIndicator = Color.white;
        }
        else
        {
            Uivm.LapIndicator = Color.orangeRed;
        }

        Uivm.Lap = Cart.Laps.Count;

        if (InPitRequest)
        {
            InPitRequest = false;
        }
    }
    
    private void SwitchMenu()
    {
        Uivm.ShowMenu = Uivm.ShowMenu switch
        {
            Visibility.Visible => Visibility.Hidden,
            Visibility.Hidden => Visibility.Visible,
        };
    }
}

public interface IVelocityProvider
{
    Vector3 Velocity { get; }
}