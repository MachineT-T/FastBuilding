using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

public class Export : MonoBehaviour
{
    public void ExportBlocks()
    {
        //将场景数据转换成json格式
        string json = JsonConvert.SerializeObject(Scene.getExportData());
        //获取保存的路径
        string path = EditorUtility.SaveFilePanel("Export Data", "", "ExportData.json", "json");
        //如果未选择路径则直接返回
        if (path.Length == 0)
        {
            return;
        }
        //尝试向文件中写入数据
        try
        {
            File.WriteAllText(path, json);
            Debug.Log("储存成功");
        }
        catch (System.Exception e)
        {
            Debug.Log("储存失败");
        }
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
