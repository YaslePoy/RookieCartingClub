using System;
using RookieCartingClub.Authoring;
using UnityEngine;

namespace RookieCartingClub.Determ
{
    public class DetermStart : MonoBehaviour
    {
        public void Start()
        {
            SessionSetup.RequestedSession = new LocalSession();
            new AutoBootstrap().Initialize("");
        }
    }
}