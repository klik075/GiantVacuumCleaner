using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "object/UpgradeContainer")]
public class UpgradeContainer : ScriptableObject
{
    public int cost;
    public float value;
}
