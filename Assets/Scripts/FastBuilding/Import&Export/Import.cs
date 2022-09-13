using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class Import : MonoBehaviour
{
    public void ImportBlocks()
    {
        //获取导入的数据文件路径
        string path = EditorUtility.OpenFilePanel("Import Data", "", "json");
        //如果未选择文件则直接返回
        if (path.Length == 0)
        {
            return;
        }
        //向文件中读取数据
        string json = File.ReadAllText(path);
        //将数据转化回SaveData对象
        SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
        Scene.ImportData(data);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
