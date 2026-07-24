using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public class FBXToPreFab : EditorWindow
{
    [MenuItem("Tools/FBX To Prefab")]
    public static void ShowWindow()
    {
        GetWindow<FBXToPreFab>("FBX To Prefab");
    }

    private string fbxFolderPath = "Assets/@Resources/background/Test";
    private string prefabFolderPath = "Assets/@Resources/Prefabs/Place";
    private GameObject templatePrefab; // 템플릿 프리팹 GameObject

    private void OnGUI()
    {
        GUILayout.Label("FBX Folder Path", EditorStyles.boldLabel);
        fbxFolderPath = EditorGUILayout.TextField(fbxFolderPath);

        GUILayout.Label("Prefab Folder Path", EditorStyles.boldLabel);
        prefabFolderPath = EditorGUILayout.TextField(prefabFolderPath);

        GUILayout.Label("Template Prefab", EditorStyles.boldLabel);
        templatePrefab = (GameObject)EditorGUILayout.ObjectField(templatePrefab, typeof(GameObject), false);

        if (GUILayout.Button("Convert FBX to Prefab"))
        {
            ConvertFBXToPrefab();
        }
    }

    private void ConvertFBXToPrefab()
    {
        if (string.IsNullOrEmpty(fbxFolderPath) || string.IsNullOrEmpty(prefabFolderPath) || templatePrefab == null)
        {
            Debug.LogError("FBX Folder Path, Prefab Folder Path, and Template Prefab must be set.");
            return;
        }

        string[] fbxFiles = Directory.GetFiles(fbxFolderPath, "*.fbx", SearchOption.AllDirectories);

        foreach (string fbxFile in fbxFiles)
        {
            GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(fbxFile);
            if (gameObject != null)
            {
                Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
                foreach (Transform childTransform in childTransforms)
                {
                    if (childTransform != gameObject.transform)
                    {
                        string parentFolderName = Path.GetFileNameWithoutExtension(fbxFile);
                        string prefabPath = Path.Combine(prefabFolderPath, parentFolderName, childTransform.name + ".prefab");

                        // 기존 프리팹 파일 삭제
                        if (File.Exists(prefabPath))
                        {
                            AssetDatabase.DeleteAsset(prefabPath);
                            Debug.Log("Deleted existing prefab: " + prefabPath);
                        }

                        GameObject childGameObject = Instantiate(templatePrefab);
                        childGameObject.name = childTransform.name;
                        //GameObject childGameObject = new GameObject(childTransform.name);
                        childGameObject.transform.position = Vector3.zero;
                        childGameObject.transform.rotation = childTransform.rotation;
                        childGameObject.transform.localScale = childTransform.localScale;

                        //// 템플릿 프리팹의 컴포넌트 복사
                        //CopyComponents(templatePrefab, childGameObject);

                        // MeshFilter 컴포넌트가 있는 경우 값을 복사
                        MeshFilter originalMeshFilter = childTransform.GetComponent<MeshFilter>();
                        if (originalMeshFilter != null)
                        {
                            childGameObject.AddComponent<MeshFilter>().sharedMesh = originalMeshFilter.sharedMesh;
                        }

                        // MeshRenderer 컴포넌트가 있는 경우 값을 복사
                        MeshRenderer originalMeshRenderer = childTransform.GetComponent<MeshRenderer>();
                        if (originalMeshRenderer != null)
                        {
                            childGameObject.AddComponent<MeshRenderer>().sharedMaterials = originalMeshRenderer.sharedMaterials;
                        }

                        BoxCollider boxCollider = childGameObject.AddComponent<BoxCollider>();
                        if (originalMeshFilter != null)
                        {
                            boxCollider.center = originalMeshFilter.sharedMesh.bounds.center;
                            boxCollider.size = originalMeshFilter.sharedMesh.bounds.size;
                        }

                        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
                        PrefabUtility.SaveAsPrefabAsset(childGameObject, prefabPath);
                        Debug.Log("Created prefab: " + prefabPath);

                        DestroyImmediate(childGameObject);
                    }
                }
            }
        }
    }
}