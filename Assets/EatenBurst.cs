using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatenBurst : SpawnEffect
{

    public override void Awake()
    {
        base.Awake();
    }


    public override void Start()
    {
        main.simulationSpeed = 1.6f;
        base.Start();
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
