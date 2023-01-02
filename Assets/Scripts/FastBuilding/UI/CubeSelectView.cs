using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VR_ChuangKe.Share;

public class CubeSelectView : MonoBehaviour
{
    public static string currentCube;
    public Button btnAll;
    public Button btnMat;
    public Button btnCol;
    public GameObject cubePref;
    public Transform content;
    //是否正在等待选择
    public static bool waitForSelect = false;
    //是否已经选择
    public static bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        //为按钮添加监听事件
        btnAll.onClick.AddListener(onClickBtnAll);
        btnMat.onClick.AddListener(onClickBtnMat);
        btnCol.onClick.AddListener(onClickBtnCol);
        //获取所有的方块
        CubeAssetObj[] cubes = AssetLoadBehaviorManager.Instance.getCubeList();
        for (int i = 0; i < cubes.Length; i++)
        {
            //实例化一个方块
            GameObject cube = Instantiate(cubePref, content);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = Vector3.one;
            //隐藏选中效果
            cube.transform.Find("Select").gameObject.SetActive(false);
            //设置方块和点击事件
            CubeBehavior cubeBehavior = cube.GetComponent<CubeBehavior>();
            cubeBehavior.cubeObj = cubes[i];
            cubeBehavior.onClick = onSelectCube;
            //显示方块图片和名称并绑定事件
            cubeBehavior.Init();
            cube.SetActive(true);
        }
        //初始默认选中所有方块的列表
        btnAll.transform.Find("Select").gameObject.SetActive(true);
        btnMat.transform.Find("Select").gameObject.SetActive(false);
        btnCol.transform.Find("Select").gameObject.SetActive(false);
    }

    void Update()
    {
        //如果不在等待则复原选择判断
        if (!waitForSelect)
        {
            selected = false;
        }
    }

    private void onSelectCube(string selectCube)
    {
        if (selectCube.Replace("Cube_", "") == currentCube) return;
        currentCube = selectCube.Replace("Cube_", "");
        for (int i = 0; i < content.childCount; i++)
        {
            Transform cube = content.GetChild(i);
            CubeBehavior cubeBehavior = cube.GetComponent<CubeBehavior>();
            cube.transform.Find("Select").gameObject.SetActive(cubeBehavior.cubeObj.name == selectCube);
            if (cubeBehavior.cubeObj.name == selectCube)
            {
                BuildMode.setMat(ResLibaryMgr.Instance.GetMatiral(currentCube));
                //如果正在等待选择，则设置已选择
                if (waitForSelect)
                {
                    selected = true;
                }
            }
        }
    }

    private void onClickBtnAll()
    {
        btnAll.transform.Find("Select").gameObject.SetActive(true);
        btnMat.transform.Find("Select").gameObject.SetActive(false);
        btnCol.transform.Find("Select").gameObject.SetActive(false);
        for (int i = 0; i < content.childCount; i++)
        {
            Transform cube = content.GetChild(i);
            CubeBehavior cubeBehavior = cube.GetComponent<CubeBehavior>();
            cube.gameObject.SetActive(true);
        }
    }

    private void onClickBtnMat()
    {
        btnAll.transform.Find("Select").gameObject.SetActive(false);
        btnMat.transform.Find("Select").gameObject.SetActive(true);
        btnCol.transform.Find("Select").gameObject.SetActive(false);
        for (int i = 0; i < content.childCount; i++)
        {
            Transform cube = content.GetChild(i);
            CubeBehavior cubeBehavior = cube.GetComponent<CubeBehavior>();
            cube.gameObject.SetActive(cubeBehavior.cubeObj.type == "CubeDatum");
        }
    }

    private void onClickBtnCol()
    {
        btnAll.transform.Find("Select").gameObject.SetActive(false);
        btnMat.transform.Find("Select").gameObject.SetActive(false);
        btnCol.transform.Find("Select").gameObject.SetActive(true);
        for (int i = 0; i < content.childCount; i++)
        {
            Transform cube = content.GetChild(i);
            CubeBehavior cubeBehavior = cube.GetComponent<CubeBehavior>();
            cube.gameObject.SetActive(cubeBehavior.cubeObj.type == "CubeColour");
        }
    }
}
