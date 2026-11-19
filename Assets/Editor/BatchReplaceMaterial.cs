using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BatchReplaceMaterial : EditorWindow
{
    private Material newMaterial;
    private List<GameObject> targetPrefabs = new List<GameObject>();
    private Vector2 scrollPos;

    [MenuItem("Tools/批量修复预制体中 Sprite 材质")]
    public static void ShowWindow()
    {
        GetWindow<BatchReplaceMaterial>("批量修复预制体中 Sprite 材质");
    }

    private void OnGUI()
    {
        GUILayout.Label("1. 拖入新材质球", EditorStyles.boldLabel);
        newMaterial = (Material)EditorGUILayout.ObjectField("新材质:", newMaterial, typeof(Material), false);

        GUILayout.Space(10);
        GUILayout.Label("2. 拖入需要修改的预制体", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));

        int removeIndex = -1;
        for (int i = 0; i < targetPrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            targetPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(targetPrefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                removeIndex = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex != -1)
        {
            targetPrefabs.RemoveAt(removeIndex);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加空槽位"))
        {
            targetPrefabs.Add(null);
        }
        if (GUILayout.Button("清空列表"))
        {
            targetPrefabs.Clear();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 40.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "将预制体拖入此区域");

        if (evt.type == EventType.DragUpdated && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }
        else if (evt.type == EventType.DragPerform && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.AcceptDrag();
            foreach (var obj in DragAndDrop.objectReferences)
            {
                GameObject go = obj as GameObject;
                if (go != null)
                {
                    targetPrefabs.Add(go);
                }
            }
            evt.Use();
        }

        GUILayout.Space(20);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("开始替换", GUILayout.Height(40)))
        {
            if (newMaterial == null)
            {
                EditorUtility.DisplayDialog("错误", "请先拖入新材质球！", "确定");
                return;
            }

            if (targetPrefabs.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "请至少拖入一个预制体！", "确定");
                return;
            }

            ReplaceMaterials();
        }
        GUI.backgroundColor = Color.white;
    }

    private void ReplaceMaterials()
    {
        int modifiedCount = 0;

        foreach (var prefab in targetPrefabs)
        {
            if (prefab == null) continue;

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(prefabPath)) continue;

            GameObject contents = PrefabUtility.LoadPrefabContents(prefabPath);

            SpriteRenderer[] renderers = contents.GetComponentsInChildren<SpriteRenderer>(true);
            bool prefabModified = false;

            foreach (var sr in renderers)
            {
                sr.sharedMaterial = newMaterial;
                prefabModified = true;
            }

            if (prefabModified)
            {
                PrefabUtility.SaveAsPrefabAsset(contents, prefabPath);
                modifiedCount++;
            }

            PrefabUtility.UnloadPrefabContents(contents);
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"批量替换完成！共修改了 {modifiedCount} 个预制体。", "确定");
    }
}