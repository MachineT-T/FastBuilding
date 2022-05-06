using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chunk))]
public class ChunkEdit : Editor
{
    private Chunk cb;
    private void OnEnable()
    {
        cb = target as Chunk;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUILayout.TextField("数据加载状态:" + cb.status);

        GUILayout.Space(10);
        GUILayout.TextField("数据加载进度");
        GUILayout.HorizontalSlider(Mathf.Clamp01(cb.mPrograss), 0, 1);

        GUILayout.Space(10);
        GUILayout.TextField("面片加载进度");
        GUILayout.HorizontalSlider(Mathf.Clamp01(cb.vPrograss), 0, 1);

        GUILayout.Space(10);
        GUILayout.TextField("渲染任务数量:" + cb.viewTask);

        GUILayout.Space(10);
        GUILayout.TextField("总的进度");
        GUILayout.HorizontalSlider(Mathf.Clamp01(cb.prograss), 0, 1);
    }
}

