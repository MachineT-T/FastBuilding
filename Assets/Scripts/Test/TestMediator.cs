using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VR_ChuangKe.Share;
using VR_ChuangKe.Share.Map;
using VR_ChuangKe.Share.Map.Comp;

namespace FastBuild.Test
{
    public class TestMediator : Frame.Ctrl.BaseMediator
    {
        public override List<string> ListExcute
        {
            get
            {
                if (_elist == null)
                {
                    _elist = new List<string>();
                    _elist.Add("SizeChange");
                    _elist.Add("SkinChange");
                    _elist.Add("RefrashMesh");
                }
                return _elist;
            }
        }
        private List<string> _elist;
        public override List<string> ListTryGetValue
        {
            get
            {
                if (_tglist == null)
                {
                    _tglist = new List<string>();
                }
                return _tglist;
            }
        }
        private List<string> _tglist;
        private int curSize;
        private int leftX;
        private int rightX;
        private int forwardZ;
        private int backZ;
        private int TopY;
        private int buttonY;
        private string curSkin;
        private string curBone;
        public static Vector3 _offestPos = Vector3.zero;
        private TsetBlockBehavior blockBehavior;
        private Dictionary<string, Dictionary<string, BoneTask>> taskDic;
        private Dictionary<string, SkeletonData> skinDic;
        protected override void OnEnd()
        {

        }

        protected override void OnEnter(object[] data)
        {
            curSize = 50;
            leftX = -curSize / 2;
            rightX = curSize % 2 == 0 ? curSize / 2 : curSize / 2 + 1;
            TopY = curSize;
            buttonY = 0;
            forwardZ = curSize % 2 == 0 ? curSize / 2 : curSize / 2 + 1;
            backZ = -curSize / 2;
            string content = ResLibaryMgr.Instance.GetTextAsset("comp_BiLi_new");
            CompObj comp = CompConfiguration.GetCompObj(content);
            skinDic.Clear();
            if (comp != null)
            {
                for (int i = 0; i < comp.sData.Count; i++)
                {
                    string bcontent = ResLibaryMgr.Instance.GetTextAsset(comp.sData[i].skeletonName);
                    SkeletonData sd = BoneManager.AnalysisSkeletonData(bcontent);
                    skinDic[comp.sData[i].Name] = sd;
                }
            }
            Loom.QueueOnAsyncThread(() =>
            {
                curSkin = comp.sData[0].Name;
                curBone = skinDic[curSkin].BoneNodeList[0].mNodeName;
                blockBehavior = new TsetBlockBehavior(leftX, rightX, TopY, buttonY, forwardZ, backZ, skinDic);
                bool initOffestPos = false;
                for (int i = 0; i < comp.sData.Count; i++)
                {
                    StatusData _sData = comp.sData[i];
                    CompCube[] ccs = _sData.cubes == null ? new CompCube[0] : _sData.cubes.ToArray();
                    for (int k = 0; k < ccs.Length; k++)
                    {

                        if (ccs[k] == null)
                            continue;
                        Vector3 ccops = ccs[k].mNodePos.getVector3();
                        if (!initOffestPos)
                        {
                            _offestPos = new Vector3(ccops.x - (Mathf.RoundToInt(ccops.x)),
                                                       ccops.y - (Mathf.RoundToInt(ccops.y)),
                                                       ccops.z - (Mathf.RoundToInt(ccops.z)));
                            initOffestPos = true;
                        }
                        Vector3 _lp = MapTool.getTransformationkPos(_offestPos, ccops, Vector3.one);
                        Vector3 _lp2 = _lp - _offestPos;
                        int x = Mathf.RoundToInt(_lp2.x);
                        int y = Mathf.RoundToInt(_lp2.y);
                        int z = Mathf.RoundToInt(_lp2.z);

                        var block = blockBehavior[x, y, z];
                        if (block != null && block.boneBlockDic[_sData.Name].ContainsKey(ccs[k].parent_ID))
                        {
                            block.boneBlockDic[_sData.Name][ccs[k].parent_ID].isCube = true;
                            block.boneBlockDic[_sData.Name][ccs[k].parent_ID].mat = ccs[k].mat_ID;
                        }
                    }
                }
                Loom.QueueOnMainThread((param) =>
                {
                    for (int i = 0; i < comp.sData.Count; i++)
                    {
                        _offestPos = Vector3.zero;
                        SkeletonData sd = skinDic[comp.sData[i].Name];
                        taskDic[comp.sData[i].Name] = new Dictionary<string, BoneTask>();
                        for (int j = 0; j < sd.BoneNodeList.Count; j++)
                        {
                            GameObject go = new GameObject(sd.BoneNodeList[j].mNodeName);
                            go.AddComponent<MeshFilter>();
                            go.AddComponent<MeshRenderer>();
                            taskDic[comp.sData[i].Name][sd.BoneNodeList[j].mNodeName] = new BoneTask(go, blockBehavior);
                        }
                    }
                    int meshIndex = 0;
                    foreach (var skin in taskDic)
                    {
                        foreach (var bone in skin.Value.Values)
                        {
                            meshIndex++;
                            bone.bone.SetActive(skin.Key == curSkin);
                        }
                    }
                   
                    foreach (var skin in taskDic)
                    {
                        foreach (var bone in skin.Value)
                        {
                            
                            bone.Value.CreateVirsual(skin.Key, bone.Key, ()=>
                            {
                                meshIndex--;
                                if (meshIndex == 0)
                                {
                                    Frame.View.UIManager.Instance.ShowUIForm("TestView", "RefrashComp", new List<string>(skinDic.Keys));
                                    Frame.View.UIManager.Instance.Excute("SkinChange", skinDic[curSkin]);
                                }
                            });
                        }
                    }
                    
                    if (param != null)
                        param();
                });
            });
        }

        protected override void OnExcute(string msg, object[] body)
        {
            if (msg == "SizeChange")
            {
                curSize = (int)body[0];
                int _leftX = -curSize / 2;
                int _rightX = curSize % 2 == 0 ? curSize / 2 : curSize / 2 + 1;
                int _TopY = curSize;
                int _buttonY = 0;
                int _forwardZ = curSize % 2 == 0 ? curSize / 2 : curSize / 2 + 1;
                int _backZ = -curSize / 2;
                Loom.QueueOnAsyncThread(() =>
                {
                    blockBehavior.RefrashSize(_leftX, _rightX, _TopY, _buttonY, _forwardZ, _backZ, skinDic);
                    Loom.QueueOnMainThread((param) =>
                    {
                        Frame.View.UIManager.Instance.Excute("SizeChange");
                        if (leftX != _leftX || rightX != _rightX || TopY != _TopY || buttonY != _buttonY || forwardZ != _forwardZ || backZ != _backZ)
                        {
                            foreach (var skin in taskDic)
                            {
                                foreach (var bone in skin.Value)
                                {
                                    bone.Value.CreateVirsual(skin.Key, bone.Key,null);
                                }
                            }
                        }
                        leftX = _leftX;
                        rightX = _rightX;
                        TopY = _TopY;
                        buttonY = _buttonY;
                        forwardZ = _forwardZ;
                        backZ = _backZ;
                        if (param != null)
                            param();
                    });
                });

            }
            else if (msg == "SkinChange")
            {
                curSkin = (string)body[0];
                foreach (var skin in taskDic)
                {
                    foreach (var bone in skin.Value.Values)
                    {
                        bone.bone.SetActive(skin.Key == curSkin);
                    }
                }
                Frame.View.UIManager.Instance.Excute("SkinChange", skinDic[curSkin]);
            }
        }

        protected override void OnInitialized(object[] data)
        {
            taskDic = new Dictionary<string, Dictionary<string, BoneTask>>();
            skinDic = new Dictionary<string, SkeletonData>();
        }

        protected override void OnRelease()
        {

        }
    }

    public class TsetBlockBehavior
    {
        public int leftX { get; private set; }
        public int rightX { get; private set; }
        public int forwardZ { get; private set; }
        public int backZ { get; private set; }
        public int TopY { get; private set; }
        public int buttonY { get; private set; }
        private Dictionary<string, TsetBlock> blockDict;
        public TsetBlock this[int x, int y, int z]
        {
            get
            {
                if (x < leftX || x > rightX ||
                    y < buttonY || y > TopY ||
                    z < backZ || z > forwardZ)
                {
                    return null;
                }
                string block_ID = string.Format("{0}-{1}-{2}", x.ToString(), y.ToString(), z.ToString());
                TsetBlock block = null;// blocks[_x, _y, _z];
                blockDict.TryGetValue(block_ID, out block);
                if (block == null)
                {
                    block = new TsetBlock(x, y, z);
                }
                return block;
            }
            set
            {
                if (x < leftX || x > rightX ||
                   y < buttonY || y > TopY ||
                   z < backZ || z > forwardZ)
                {
                    return;
                }
                string block_ID = string.Format("{0}-{1}-{2}", x.ToString(), y.ToString(), z.ToString());
                blockDict[block_ID] = value;
            }
        }

        public TsetBlockBehavior(int _leftX, int _rightX, int _TopY, int _buttonY, int _forwardZ, int _backZ, Dictionary<string, SkeletonData> skinDic)
        {
            leftX = _leftX;
            rightX = _rightX;
            TopY = _TopY;
            buttonY = _buttonY;
            forwardZ = _forwardZ;
            backZ = _backZ;
            blockDict = new Dictionary<string, TsetBlock>();
            for (int y = this.buttonY; y <= this.TopY; y++)
            {
                for (int x = this.leftX; x <= this.rightX; x++)
                {
                    for (int z = this.backZ; z <= this.forwardZ; z++)
                    {
                        string block_ID = string.Format("{0}-{1}-{2}", x.ToString(), y.ToString(), z.ToString());
                        TsetBlock block = block = new TsetBlock(x, y, z);
                        blockDict[block_ID] = block;
                        foreach (var item in skinDic)
                        {
                            foreach (var bn in item.Value.BoneNodeList)
                            {
                                if (!block.boneBlockDic.ContainsKey(item.Key))
                                    block.boneBlockDic[item.Key] = new Dictionary<string, Block1>();
                                if (!block.boneBlockDic[item.Key].ContainsKey(bn.mNodeName))
                                    block.boneBlockDic[item.Key][bn.mNodeName] = new Block1();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 大小变动
        /// </summary>
        /// <param name="_leftX"></param>
        /// <param name="_rightX"></param>
        /// <param name="_TopY"></param>
        /// <param name="_buttonY"></param>
        /// <param name="_forwardZ"></param>
        /// <param name="_backZ"></param>
        public void RefrashSize(int _leftX, int _rightX, int _TopY, int _buttonY, int _forwardZ, int _backZ, Dictionary<string, SkeletonData> skinDic)
        {
            int lx = Mathf.Min(leftX, _leftX);
            int rx = Mathf.Max(rightX, _rightX);
            int ty = Mathf.Max(TopY, _TopY);
            int dy = Mathf.Min(buttonY, _buttonY);
            int fz = Mathf.Max(forwardZ, _forwardZ);
            int bz = Mathf.Min(backZ, _backZ);

            for (int y = dy; y <= ty; y++)
            {
                for (int x = lx; x <= rx; x++)
                {
                    for (int z = bz; z <= fz; z++)
                    {
                        if (x >= _leftX && x <= _rightX && y >= _buttonY && y <= _TopY && z >= _backZ && z <= _forwardZ)
                        {
                            string block_ID = string.Format("{0}-{1}-{2}", x.ToString(), y.ToString(), z.ToString());
                            if (!blockDict.ContainsKey(block_ID))
                            {
                                TsetBlock block = block = new TsetBlock(x, y, z);
                                blockDict[block_ID] = block;
                                foreach (var item in skinDic)
                                {
                                    foreach (var bn in item.Value.BoneNodeList)
                                    {
                                        if (!block.boneBlockDic.ContainsKey(item.Key))
                                            block.boneBlockDic[item.Key] = new Dictionary<string, Block1>();
                                        if (!block.boneBlockDic[item.Key].ContainsKey(bn.mNodeName))
                                            block.boneBlockDic[item.Key][bn.mNodeName] = new Block1();
                                    }
                                }
                            }
                        }
                        else
                        {
                            string block_ID = string.Format("{0}-{1}-{2}", x.ToString(), y.ToString(), z.ToString());
                            blockDict.Remove(block_ID);
                        }
                    }
                }
            }

            leftX = _leftX;
            rightX = _rightX;
            TopY = _TopY;
            buttonY = _buttonY;
            forwardZ = _forwardZ;
            backZ = _backZ;
        }
    }

    public class TsetBlock
    {
        public int x;
        public int y;
        public int z;


        public Dictionary<string, Dictionary<string, Block1>> boneBlockDic { get; private set; }
        public TsetBlock(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            boneBlockDic = new Dictionary<string, Dictionary<string, Block1>>();
        }
    }

    public class Block1
    {

        /// <summary>
        /// 存在方块
        /// </summary>
        public bool isCube;
        /// <summary>
        /// 存在骨骼点
        /// </summary>
        public bool isBone;
        /// <summary>
        /// 骨骼点是否显示出来
        /// </summary>
        public bool isBoneDisplay;

        public string mat;
    }

    /// <summary>
    /// 节点任务
    /// </summary>
    public class BoneTask
    {
        public GameObject bone { get; private set; }
        TsetBlockBehavior blocks;
        MeshTask meshTask;
        public BoneTask(GameObject bone, TsetBlockBehavior blocks)
        {
            this.blocks = blocks;
            this.bone = bone;
            meshTask = new MeshTask(bone, blocks);
        }

        /// <summary>
        /// 渲染
        /// </summary>
        public void CreateVirsual(string skin, string bn,System.Action callback)
        {
            if (meshTask.IsWorking)
            {
                meshTask.Stop();
                meshTask = new MeshTask(bone, blocks);
            }
            meshTask.Start(skin, bn, callback);
        }

        public void Release()
        {
            GameObject.Destroy(bone);
        }

    }

    /// <summary>
    /// 渲染任务
    /// </summary>
    public class MeshTask
    {
        int unit = 1;
        private float voxelUVThresh = 0.0001f;
        public bool IsWorking { get; private set; }
        private object lockobj;
        private long taskId;
        private TsetBlockBehavior blocks;
        GameObject bone;
        private Dictionary<string, MaterialMesh> materialDic;
        public MeshTask(GameObject bone, TsetBlockBehavior blocks)
        {
            materialDic = new Dictionary<string, MaterialMesh>();
            lockobj = new object();
            IsWorking = false;
            taskId = System.Guid.NewGuid().ToString().GetHashCode();
            this.blocks = blocks;
            this.bone = bone;
        }

        /// <summary>
        /// 开始渲染
        /// </summary>
        public void Start(string skin, string bn, System.Action callback)
        {
            IsWorking = true;
            Loom.QueueOnAsyncThread(taskId, () =>
             {

                 for (int y = this.blocks.buttonY; y < this.blocks.TopY; y++)
                 {
                     for (int x = this.blocks.leftX; x < this.blocks.rightX; x++)
                     {
                         for (int z = this.blocks.backZ; z < this.blocks.forwardZ; z++)
                         {
                             if (!IsWorking)
                                 break;
                             var block = this.blocks[x, y, z];
                             if (block != null && (block.boneBlockDic[skin][bn].isCube || (block.boneBlockDic[skin][bn].isBone && block.boneBlockDic[skin][bn].isBoneDisplay)) && !string.IsNullOrEmpty(block.boneBlockDic[skin][bn].mat))
                             {
                                 MaterialMesh materialMesh = null;
                                 if (!materialDic.ContainsKey(block.boneBlockDic[skin][bn].mat))
                                 {
                                     lock (lockobj)
                                     {
                                         materialMesh = new MaterialMesh(lockobj, block.boneBlockDic[skin][bn].mat);
                                         materialDic[block.boneBlockDic[skin][bn].mat] = materialMesh;
                                     }
                                 }
                                 else
                                 {
                                     materialMesh = materialDic[block.boneBlockDic[skin][bn].mat];
                                 }
                                 if (!IsTransparent(skin, bn, x - unit, y, z))
                                     materialMesh.BuildFace(4, new Vector3(x, y, z), Vector3.up, Vector3.forward, false);
                                 // Right wall
                                 if (!IsTransparent(skin, bn, x + unit, y, z))
                                     materialMesh.BuildFace(5, new Vector3(x + unit, y, z), Vector3.up, Vector3.forward, true);

                                 // Bottom wall
                                 if (!IsTransparent(skin, bn, x, y - unit, z))
                                     materialMesh.BuildFace(1, new Vector3(x, y, z), Vector3.forward, Vector3.right, false);
                                 // Top wall
                                 if (!IsTransparent(skin, bn, x, y + unit, z))
                                     materialMesh.BuildFace(0, new Vector3(x, y + unit, z), Vector3.forward, Vector3.right, true);

                                 // Back
                                 if (!IsTransparent(skin, bn, x, y, z - unit))
                                     materialMesh.BuildFace(3, new Vector3(x, y, z), Vector3.up, Vector3.right, true);
                                 // Front
                                 if (!IsTransparent(skin, bn, x, y, z + unit))
                                     materialMesh.BuildFace(2, new Vector3(x, y, z + unit), Vector3.up, Vector3.right, false);
                             }
                         }
                     }
                 }
                 Loom.QueueOnMainThread(taskId, (param) =>
                 {
                     List<MaterialMesh> meshes = new List<MaterialMesh>(materialDic.Values);
                     for (int i = 0; i < meshes.Count; i++)
                     {
                         meshes[i].BuildFace();
                     }
                     CombineInstance[] combineInstances = new CombineInstance[meshes.Count];
                     Material[] mats = new Material[meshes.Count];
                     for (int i = 0; i < meshes.Count; i++)
                     {
                         mats[i] = ResLibaryMgr.Instance.GetMatiral(meshes[i].material);
                         combineInstances[i].mesh = meshes[i].visualMesh;                   //将共享mesh，赋值
                         combineInstances[i].transform = bone.transform.localToWorldMatrix; //本地坐标转矩阵，赋值
                     }

                     bone.GetComponent<MeshFilter>().mesh = new Mesh();
                     bone.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances, false);//为mesh.CombineMeshes添加一个 false 参数，表示并不是合并为一个网格，而是一个子网格列表
                     bone.GetComponent<MeshRenderer>().sharedMaterials = mats;
                     IsWorking = false;
                     if (callback != null)
                         callback();
                     if (param != null)
                         param();
                 });
             });
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (IsWorking)
            {
                IsWorking = false;
                Loom.StopOnMainThread(taskId);
                Loom.StopOnAsyncThread(taskId);
            }
        }
        private bool IsTransparent(string skin, string bn, int x, int y, int z)
        {
            var block = this.blocks[x, y, z];
            if (block != null)
            {
                return (block.boneBlockDic[skin][bn].isCube || (block.boneBlockDic[skin][bn].isBone && block.boneBlockDic[skin][bn].isBoneDisplay)) && !string.IsNullOrEmpty(block.boneBlockDic[skin][bn].mat);
            }
            return false;
        }
    }


    public class MaterialMesh
    {
        private float voxelUVThresh = 0.0001f;
        public Mesh visualMesh { get; private set; }
        public string material { get; private set; }
        private object lockobj;
        private List<int> tris = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Vector3> verts = new List<Vector3>();
        public MaterialMesh(object lockobj, string material)
        {
            this.lockobj = lockobj;
            this.material = material;
        }

        public void BuildFace(byte direct, Vector3 _corner, Vector3 up, Vector3 right, bool reversed)
        {
            lock (lockobj)
            {
                _corner = MapTool.getTransformationkPos(TestMediator._offestPos, _corner, Vector3.one );
                int index = verts.Count;
                Vector3 corner = _corner;
                corner.x -= 0.5f;
                corner.y -= 0.5f;
                corner.z -= 0.5f;

                verts.Add(corner);
                verts.Add(corner + up);
                verts.Add(corner + up + right);
                verts.Add(corner + right);

                uvs.Add(new Vector2(direct * 1.0f / 6.0f + voxelUVThresh, voxelUVThresh));
                uvs.Add(new Vector2(direct * 1.0f / 6.0f + voxelUVThresh, 1 - voxelUVThresh));
                uvs.Add(new Vector2((direct + 1) * 1.0f / 6.0f - voxelUVThresh, 1 - voxelUVThresh));
                uvs.Add(new Vector2((direct + 1) * 1.0f / 6.0f - voxelUVThresh, 0 + voxelUVThresh));

                if (reversed)
                {
                    tris.Add(index + 0);
                    tris.Add(index + 1);
                    tris.Add(index + 2);
                    tris.Add(index + 2);
                    tris.Add(index + 3);
                    tris.Add(index + 0);
                }
                else
                {
                    tris.Add(index + 1);
                    tris.Add(index + 0);
                    tris.Add(index + 2);
                    tris.Add(index + 3);
                    tris.Add(index + 2);
                    tris.Add(index + 0);
                }
            }
        }

        public void BuildFace()
        {
            lock (lockobj)
            {
                visualMesh = new Mesh();
                visualMesh.Clear();
                visualMesh.vertices = verts.ToArray();
                visualMesh.uv = uvs.ToArray();
                visualMesh.triangles = tris.ToArray();
                visualMesh.RecalculateBounds();
                visualMesh.RecalculateNormals();
            }

        }
    }
}
