using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VR_ChuangKe.Share.Map
{
    [CustomEditor(typeof(MaterialSetting))]
    public class MaterialSettingEditor : Editor
    {
        [MenuItem("Assets/Create MaterialSetting")]
        static void CreateMaterialSetting()
        {
            MaterialSetting config = ScriptableObject.CreateInstance<MaterialSetting>();
            config.materials = new Material[0];

            AssetDatabase.CreateAsset(config, "Assets/Resources/MaterialSetting.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}