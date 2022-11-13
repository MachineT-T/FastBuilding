using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using VR_ChuangKe.Share;

public class CubeBehavior : MonoBehaviour
{
    public CubeAssetObj cubeObj;

    public System.Action<string> onClick;
    public void Init()
    {
        transform.Find("Text").GetComponent<Text>().text = LanguageMgr.Instance.getTranslationValue(cubeObj.name);
        transform.Find("Image").GetComponent<RawImage>().texture = DisplayIconMgr.Instance.getTexture(cubeObj.name);
        gameObject.GetComponent<Button>().onClick.AddListener(onSelectClick);
    }

    private void onSelectClick()
    {
        onClick(cubeObj.name);
    }
}
