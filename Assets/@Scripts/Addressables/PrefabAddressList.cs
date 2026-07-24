using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class GroupAddress
{
    public string groupName;
    public List<string> addresses;
}
[CreateAssetMenu(fileName = "PrefabAddressList", menuName = "ScriptableObjects/PrefabAddressList", order = 1)]
public class PrefabAddressList : ScriptableObject
{
    public List<GroupAddress> groupAddresses = new List<GroupAddress>();
}
