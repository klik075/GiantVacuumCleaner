using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using System;

public class AddressableGroupExtractor
{
    [MenuItem("Tools/Update Prefab Address List")]
    public static void UpdatePrefabAddressList()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Debugger.LogError("AddressableAssetSettingsDefaultObject.Settings is null. Make sure Addressables are properly set up.");
            return;
        }

        //string[] groupNames = { "Thing1", "Thing2", "Thing3" };//Set Group Name
        List<string> groupNames = new List<string>();

        foreach (PrefabGroup group in Enum.GetValues(typeof(PrefabGroup)))
        {
            groupNames.Add(group.ToString());
            //Debugger.Log($"¿Ã∏ß : {group}");
        }
        var prefabAddressList = AssetDatabase.LoadAssetAtPath<PrefabAddressList>("Assets/@Scripts/Addressables/PrefabAddressList.asset");

        if (prefabAddressList == null)
        {
            prefabAddressList = ScriptableObject.CreateInstance<PrefabAddressList>();
            AssetDatabase.CreateAsset(prefabAddressList, "Assets/@Scripts/Addressables/PrefabAddressList.asset");
        }

        prefabAddressList.groupAddresses.Clear();

        foreach (var groupName in groupNames)
        {
            var group = settings.FindGroup(groupName);

            if (group == null)
            {
                Debugger.LogError($"Group '{groupName}' not found.");
                continue;
            }

            GroupAddress groupAddress = new GroupAddress();
            groupAddress.groupName = groupName;
            groupAddress.addresses = new List<string>();

            foreach (var entry in group.entries)
            {
                groupAddress.addresses.Add(entry.address);
            }

            prefabAddressList.groupAddresses.Add(groupAddress);
        }

        EditorUtility.SetDirty(prefabAddressList);
        AssetDatabase.SaveAssets();

        Debugger.Log("Prefab address list updated.");
    }
}