using UnityEngine;
using System.Collections;

namespace LuaFramework {

    /// <summary>
    /// </summary>
    public class Main : MonoBehaviour {

        void Start() {
            Loom.Initialize();
            Screen.orientation = ScreenOrientation.AutoRotation;  
            Screen.autorotateToLandscapeLeft = true;  
            Screen.autorotateToLandscapeRight = true;  
            Screen.autorotateToPortrait = false;  
            Screen.autorotateToPortraitUpsideDown = false;  
            AppFacade.Instance.StartUp();   //启动游戏
        }
    }
}