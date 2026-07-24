using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "object", menuName = "object/UpgradeList")]
public class UpgradeList : ScriptableObject
{
    public List<UpgradeContainer> timeList = new List<UpgradeContainer>();
    public List<UpgradeContainer> speedList = new List<UpgradeContainer>();
    public List<UpgradeContainer> incomeList = new List<UpgradeContainer>();
    public int ReturnNextCount(UpgradeType type, int count)
    {
        if (type == UpgradeType.Speed)
        {
            if (count + 1 >= speedList.Count)
            {
                return -1;
            }
            return count + 1;
        }
        else if (type == UpgradeType.Time)
        {
            if (count + 1>= timeList.Count)
            {
                return -1;
            }
            return count + 1;
        }
        else if (type == UpgradeType.Income)
        {
            if (count + 1 >= incomeList.Count)
            {
                return -1;
            }
            return count + 1;
        }
        else 
        {
            return -1; 
        }
    }
}
