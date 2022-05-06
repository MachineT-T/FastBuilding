using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frame.Model{

    public class FMAction<T> : FMAction
    {
        public delegate T GObject();
        public delegate T GObject<T0>(T0 arge0);
        public delegate T GObject<T0, T1>(T0 arge0, T1 arge1);
        public delegate T GObject<T0, T1, T2>(T0 arge0, T1 arge1, T2 arge2);
        public delegate T GObject<T0, T1, T2, T3>(T0 arge0, T1 arge1, T2 arge2, T3 arge3);

        public delegate T[] GArray();
        public delegate T[] GArray<T0>(T0 arge0);
        public delegate T[] GArray<T0, T1>(T0 arge0, T1 arge1);
        public delegate T[] GArray<T0, T1, T2>(T0 arge0, T1 arge1, T2 arge2);
        public delegate T[] GArray<T0, T1, T2, T3>(T0 arge0, T1 arge1, T2 arge2, T3 arge3);
    }

    public class FMAction
    {
        public delegate void VAction();
        public delegate void VAction<T0>(T0 arge0);
        public delegate void VAction<T0, T1>(T0 arge0, T1 arge1);
        public delegate void VAction<T0, T1, T2>(T0 arge0, T1 arge1, T2 arge2);
        public delegate void VAction<T0, T1, T2, T3>(T0 arge0, T1 arge1, T2 arge2, T3 arge3);
    }
}
