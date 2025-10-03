using System.Linq;
using UnityEngine;

public class MapHandle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Texture2D mapTexture;
    public UIVM Uivm;
    private GameObject Cart;
    private Transform origin;
    public float TrackHeight;
    public float TrackWidth;
    public int MapHeight;
    void Start()
    {
        Uivm.MapWidth = (int)(400.0 * (mapTexture.width / (double)mapTexture.height));
        Uivm.MapHeight = MapHeight;
        Uivm.Map = mapTexture;
        origin = GameObject.Find("TrackOrigin").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cart is null)
        {
            var carts = GameObject.FindGameObjectsWithTag("Cart");
            if (!carts.Any())
            {
                return;
            }
            
            Cart = carts.First(go => go.GetComponent<TrackPlacement>().IsOwner);
        }

        if (Cart is null)
        {
            return;
        }
        
        var delta = Cart.transform.position - origin.position;
        delta.x /= -TrackWidth;
        delta.z /= TrackHeight;

        Uivm.PlayerX = (int)(delta.x * Uivm.MapWidth);
        Uivm.PlayerY = (int)(delta.z * Uivm.MapHeight);
    }
}