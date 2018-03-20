using UnityEngine;
using System.Collections;

namespace LuaFramework {

    /// <summary>
    /// </summary>
    public class Main : MonoBehaviour {

        void Start() {
            Loom.Initialize();
            Screen.orientation = ScreenOrientation.AutoRotation;  
            AppFacade.Instance.StartUp();   //启动游戏
        }
    }
}