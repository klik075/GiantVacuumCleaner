using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thing : MonoBehaviour
{
    [SerializeField]
    private ThingData thingData;
    private ThingData currentData;

    public ThingData CurrentData { get => currentData; private set => currentData = value; }
    private void Awake()
    {
        InitData();
    }
    private void InitData()
    {
        if (thingData != null)
        {
            currentData = new ThingData();
            currentData.Lv = thingData.Lv;
            currentData.Exp = thingData.Exp;
            currentData.Size = thingData.Size;
            currentData.Gold = thingData.Gold;
            //Debugger.Log($"lv : {currentData.Lv}, Exp : {currentData.Exp}, Size : {currentData.Size}");
        }
    }
}
