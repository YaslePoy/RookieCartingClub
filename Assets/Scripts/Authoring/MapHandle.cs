using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

//todo
namespace RookieCartingClub.Authoring
{
    public class MapHandle : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public Texture2D mapTexture;
        public Texture2D enemyPoint;
        public UIVM Uivm;
        public float TrackHeight;
        public float TrackWidth;
        public int MapHeight;
        private UIDocument _document;
        private VisualElement _mapVE;
        public float3 CartPosition;
        private Transform origin;
        public CartHandleAuthoring Cart;
        public static MapHandle Instance;
    
        private void Start()
        {
            Instance = this;
            Uivm.MapWidth = (int)(MapHeight * (mapTexture.width / (double)mapTexture.height));
            Uivm.MapHeight = MapHeight;
            Uivm.Map = mapTexture;
            origin = GameObject.Find("TrackOrigin").transform;
            _document = GetComponent<UIDocument>();
            _mapVE = _document.rootVisualElement.Q("Map");
        }

        // // Update is called once per frame
        // private void UpdateMap()
        // {
        //     Cart ??= racers[0];
        //
        //     if (Cart is null) return;
        //
        //     MoveSelf();
        //     MoveEnemyPoints();
        // }

        public void MoveSelf()
        {
            var delta = (Vector3)CartPosition - origin.position;
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

        private void MoveEnemyPoints(List<CartHandleAuthoring> racers)
        {
            SyncRacerCount(racers);
            SetupPositions();
        }

        private void SyncRacerCount(List<CartHandleAuthoring> racers)
        {
            if (_mapVE.childCount != racers.Count)
            {
                if (_mapVE.childCount > racers.Count)
                {
                    var remove = _mapVE.childCount - racers.Count;
                    for (var i = 0; i < remove; i++)
                    {
                        _mapVE.RemoveAt(_mapVE.childCount - 1);
                        print("Enemy removed");
                    }
                }
                else if (_mapVE.childCount < racers.Count)
                {
                    var add = racers.Count - _mapVE.childCount;
                    for (var i = 0; i < add; i++)
                    {
                        AddEnemyPoint();
                        print("Enemy added");
                    }
                }
            }
        }

        private void SetupPositions()
        {
            // todo remake positions system
            // var positions = racers.Skip(1).ToList();
            // for (var index = 0; index < positions.Count; index++)
            // {
            //     var position = positions[index];
            //     var delta = position.transform.position - origin.position;
            //     delta.x /= -TrackWidth;
            //     delta.z /= TrackHeight;
            //
            //     var point = _mapVE[index + 1];
            //     point.style.marginLeft = new StyleLength(Length.Pixels(delta.x * Uivm.MapWidth));
            //     point.style.marginTop = new StyleLength(Length.Pixels(delta.z * Uivm.MapHeight));
            // }
        }
    }
}