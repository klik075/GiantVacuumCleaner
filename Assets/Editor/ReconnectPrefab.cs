using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


public class ReconnectPrefabEditor : EditorWindow
{
    [MenuItem("Tools/Reconnect Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<ReconnectPrefabEditor>("Reconnect Prefabs");
    }

    private GameObject mapObject;
    private string searchFolder = "Assets/@Resources/Prefabs/Place"; // 여기에 특정 폴더 경로를 설정하세요

    private void OnGUI()
    {
        GUILayout.Label("Reconnect Prefabs", EditorStyles.boldLabel);

        mapObject = (GameObject)EditorGUILayout.ObjectField("Map Object", mapObject, typeof(GameObject), true);
        searchFolder = EditorGUILayout.TextField("Search Folder", searchFolder);

        if (GUILayout.Button("Reconnect"))
        {
            if (mapObject != null)
            {
                ReconnectPrefabs(mapObject);
            }
            else
            {
                Debug.LogWarning("Map Object를 선택하세요.");
            }
        }
    }

    private void ReconnectPrefabs(GameObject mapObject)
    {
        Transform[] childTransforms = mapObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in childTransforms)
        {
            if (child != mapObject.transform)
            {
                string childName = child.gameObject.name;
                string prefabPath = FindPrefabPath(childName);

                if (!string.IsNullOrEmpty(prefabPath))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    if (prefab != null)
                    {
                        string folderPath = System.IO.Path.GetDirectoryName(prefabPath);
                        string folderName = System.IO.Path.GetFileName(folderPath);

                        GameObject folderObject = GameObject.Find(folderName);
                        if (folderObject == null)
                        {
                            folderObject = new GameObject(folderName);
                            folderObject.transform.SetParent(mapObject.transform);
                        }

                        GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        newInstance.transform.SetParent(folderObject.transform);
                        //newInstance.transform.SetSiblingIndex(folderObject.transform.GetSiblingIndex());
                        newInstance.transform.localPosition = child.localPosition;
                        newInstance.transform.localRotation = child.localRotation;
                        newInstance.transform.localScale = child.localScale;

                        DestroyImmediate(child.gameObject);
                        Debug.Log($"{childName} 프리팹으로 교체됨.");
                    }
                }
                else
                {
                    Debug.LogWarning($"{childName}에 대한 프리팹을 찾을 수 없음.");
                }
            }
        }
        SortChildrenByName(mapObject.transform);
    }

    private string FindPrefabPath(string prefabName)
    {
        string[] guids = AssetDatabase.FindAssets($"t:GameObject {prefabName}", new[] { searchFolder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == prefabName)
            {
                return path;
            }
        }
        return string.Empty;
    }
    private void SortChildrenByName(Transform parent)
    {
        // 자식 객체들을 이름 순으로 정렬
        var children = parent.Cast<Transform>().OrderBy(t => t.name).ToList();

        // 자식 객체들의 순서를 변경
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }
}