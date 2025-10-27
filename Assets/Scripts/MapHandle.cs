using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MapHandle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Texture2D mapTexture;
    public Texture2D enemyPoint;
    public UIVM Uivm;
    private GameObject Cart;
    private Transform origin;
    public float TrackHeight;
    public float TrackWidth;
    public int MapHeight;
    private UIDocument _document;
    private VisualElement _mapVE;

    void Start()
    {
        Uivm.MapWidth = (int)(MapHeight * (mapTexture.width / (double)mapTexture.height));
        Uivm.MapHeight = MapHeight;
        Uivm.Map = mapTexture;
        origin = GameObject.Find("TrackOrigin").transform;
        _document = GetComponent<UIDocument>();
        _mapVE = _document.rootVisualElement.Q("Map");
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

        MoveSelf();
        MoveEnemyPoints();
    }

    private void MoveSelf()
    {
        var delta = Cart.transform.position - origin.position;
        delta.x /= -TrackWidth;
        delta.z /= TrackHeight;

        Uivm.PlayerX = (int)(delta.x * Uivm.MapWidth);
        Uivm.PlayerY = (int)(delta.z * Uivm.MapHeight);
    }

    private void AddEnemyPoint()
    {
        var ve = new VisualElement
        {
            style =
            {
                backgroundImage = new StyleBackground(enemyPoint),
                flexGrow = 1f,
                backgroundRepeat = new StyleBackgroundRepeat(new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat)),
                backgroundPositionX =
                    new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Center)),
                backgroundPositionY =
                    new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Center)),
                backgroundSize = new StyleBackgroundSize(new BackgroundSize(Length.Percent(100), Length.Percent(100))),
                width = new StyleLength(Length.Pixels(7)),
                height = new StyleLength(Length.Pixels(7)),
                position = new StyleEnum<Position>(Position.Absolute),
                unitySliceType = new StyleEnum<SliceType>(SliceType.Sliced),
                top = 0,
                left = 0,
                marginLeft = new StyleLength(Length.Pixels(100)),
                marginTop = new StyleLength(Length.Pixels(100))
            }
        };

        _mapVE.Add(ve);
    }

    private void MoveEnemyPoints()
    {
        SyncRacerCount();
        SetupPositions();
    }

    private void SyncRacerCount()
    {
        if (_mapVE.childCount != RaceControl.Singleton.racers.Count)
        {
            if (_mapVE.childCount > RaceControl.Singleton.racers.Count)
            {
                var remove = _mapVE.childCount - RaceControl.Singleton.racers.Count;
                for (int i = 0; i < remove; i++)
                {
                    _mapVE.RemoveAt(_mapVE.childCount - 1);
                    print("Enemy removed");
                }
            }
            else if (_mapVE.childCount < RaceControl.Singleton.racers.Count)
            {
                var add = RaceControl.Singleton.racers.Count - _mapVE.childCount;
                for (int i = 0; i < add; i++)
                {
                    AddEnemyPoint();
                    print("Enemy added");
                }
            }
        }
    }

    private void SetupPositions()
    {
        var positions = RaceControl.Singleton.racers.Where(i => i.IsOwner == false).ToList();
        for (var index = 0; index < positions.Count; index++)
        {
            var position = positions[index];
            var delta = position.transform.position - origin.position;
            delta.x /= -TrackWidth;
            delta.z /= TrackHeight;

            var point = _mapVE[index + 1];
            point.style.marginLeft = new StyleLength(Length.Pixels(delta.x * Uivm.MapWidth));
            point.style.marginTop = new StyleLength(Length.Pixels(delta.z * Uivm.MapHeight));
        }
    }
}