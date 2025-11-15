using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RookieCartingClub.Components;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RookieCartingClub.Authoring
{
    public class DemoReader : MonoBehaviour
    {
        public CartHandle Cart;
        public bool IsRecotring;

        public List<CartStamp> cartStamps = new();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!IsRecotring)
                return;

            cartStamps.Add(new CartStamp
            {
                position = Cart.transform.position,
                rotation = Cart.transform.rotation
            });
        }

        public void StartRecording()
        {
            cartStamps.Clear();
            IsRecotring = true;
        }

        public byte[] Finish()
        {
            IsRecotring = false;
            var name = new FixedString64Bytes(SceneManager.GetActiveScene().name);
            var playerId = Cart.PlayerId;
            var data = new byte[cartStamps.Count * Marshal.SizeOf<CartStamp>()];

            Array.Copy(cartStamps.ToArray(), data, cartStamps.Count);

            var finalData =
                new Span<byte>(new byte[Marshal.SizeOf<FixedString64Bytes>() + Marshal.SizeOf<int>() + data.Length]);
            MemoryMarshal.Write(finalData, ref name);
            MemoryMarshal.Write(finalData[64..], ref playerId);
            data.CopyTo(finalData[(64 + 4)..]);

            return finalData.ToArray();
        }
    }
}