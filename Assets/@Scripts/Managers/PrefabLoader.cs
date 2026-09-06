using Cysharp.Threading.Tasks;
using Scripts.Framework.Managers.Asset.Core;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum PrefabGroup
{
    bathroom_random_lv0,
    bedroom_random_lv1,
    farm_random_lv4,
    farm_random_lv5,
    bg_random_lv4,
    bg_random_lv5,
    bg_random_lv5_tree,
    Kitchen_random_lv0,
    Kitchen_random_lv1,
    livingroom_random_lv0,
    livingroom_random_lv1
}
public class PrefabLoader
{
    public PrefabAddressList prefabAddressList;
    public Dictionary<string, List<GameObject>> loadedPrefabsByGroup = new Dictionary<string, List<GameObject>>();
    private string loadAssetAddress = "PrefabAddressList";
    public event Action EndInit;

    public void Init()
    {
        LoadAllPrefabs().Forget();
    }
    public async UniTask LoadAllPrefabs()
    {
        // Addressables 전용 확장 메서드 .ToUniTask() 사용 권장
        var handle = Addressables.LoadAssetAsync<PrefabAddressList>(loadAssetAddress);
        prefabAddressList = await handle.ToUniTask();

        if (prefabAddressList != null)
        {
            // ⭐ 모든 그룹을 동시에(병렬로) 비동기 로딩하여 로딩 속도 최적화
            var groupTasks = new List<UniTask>();

            foreach (PrefabGroup group in Enum.GetValues(typeof(PrefabGroup)))
            {
                groupTasks.Add(LoadPrefabsFromGroup(group));
            }

            // 모든 그룹이 로드될 때까지 한 번에 기다림
            await UniTask.WhenAll(groupTasks);

            CallEndInit();
        }
        else
        {
            Debugger.LogError("Failed to load PrefabAddressList.");
        }
    }

    //public async UniTask LoadAllPrefabs() 기존 사용 방식
    //{
    //    var handle = Addressables.LoadAssetAsync<PrefabAddressList>(loadAssetAddress);
    //    await handle.Task.AsUniTask();

    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        prefabAddressList = handle.Result;

    //        foreach (PrefabGroup group in Enum.GetValues(typeof(PrefabGroup)))
    //        {
    //            await LoadPrefabsFromGroup(group);
    //        }

    //        CallEndInit();
    //    }
    //    else
    //    {
    //        Debugger.LogError("Failed to load PrefabAddressList.");
    //    }
    //}

    private void CallEndInit()
    {
        EndInit?.Invoke();
    }

    public async UniTask LoadPrefabsFromGroup(PrefabGroup group)
    {
        string groupName = group.ToString();
        foreach (GroupAddress groupAddress in prefabAddressList.groupAddresses)
        {
            if (groupAddress.groupName == groupName)
            {
                List<UniTask> loadTasks = new List<UniTask>();
                foreach (string address in groupAddress.addresses)
                {
                    //LoadPrefab(address, groupName);
                    loadTasks.Add(LoadPrefab(address, groupName));
                }
                await UniTask.WhenAll(loadTasks);
                return;
            }
        }

        Debugger.LogError($"Group '{groupName}' does not exist in the address list.");
    }

    private async UniTask LoadPrefab(string address, string groupName)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task.AsUniTask();

        OnPrefabLoaded(handle, groupName);
        //Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> obj) =>
        //{
        //    OnPrefabLoaded(obj, groupName);
        //};
    }

    private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj, string groupName)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            if (!loadedPrefabsByGroup.ContainsKey(groupName))
            {
                loadedPrefabsByGroup[groupName] = new List<GameObject>();
            }

            loadedPrefabsByGroup[groupName].Add(obj.Result);
            //Debugger.Log($"{loadedPrefabsByGroup[groupName].Count}");
        }
        else
        {
            Debugger.LogError($"Failed to load prefab at address: {obj.DebugName}");
        }
    }
    public List<GameObject> GetGroupObject(string groupName)
    {
        if (loadedPrefabsByGroup.ContainsKey(groupName))
            return loadedPrefabsByGroup[groupName];

        return null;
    }

}
