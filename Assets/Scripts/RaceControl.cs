using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceControl : MonoBehaviour
{
    public List<CartHandle> racers = new List<CartHandle>();

    public int SessionTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void StartRace()
    {
        racers = GameObject.FindGameObjectsWithTag("Cart").ToList().Select(g => g.GetComponent<CartHandle>()).ToList();
    }
    
    // Update is called once per frame
    void Update()
    {
        racers = racers.OrderByDescending(c => c.Laps.Count).ThenByDescending(i => i.Laps.Last().Count).ToList();
        var delta = SessionTime * 60 - (int)Time.time;
        delta = delta > 0 ? delta : 0;
        var viewTime = TimeSpan.FromSeconds(delta).ToString("mm:ss");
    }
}
