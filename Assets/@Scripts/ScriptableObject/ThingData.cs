using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "object/ThingData")]
public class ThingData : ObjectBase
{
    [SerializeField]
    private int _gold;
    public int Gold { get => _gold; set => _gold = value; }
}
