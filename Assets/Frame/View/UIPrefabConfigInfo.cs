
using System.Collections.Generic;
using System;

namespace Frame.View
{
    [Serializable]
    public class UIPrefabConfigInfo {
        public List<UIPrefabConfigNode> UIPrefabInfo = null;
	}

	[Serializable]
	public class UIPrefabConfigNode
    {
        public bool UIFormLuaScript = false;
        public string UIFormName = null;
		public string UIFormClassName = null;
        public string UIFormPrefabName = null;
    }
}