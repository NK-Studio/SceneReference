# SceneReference
유니티에서 씬 에셋을 참조하려는 의도가 있다면, SceneReference를 사용하세요.

유니티에서 제공하는 `Scene`Class는 인스펙터에 노출되지 않으며, `SceneAsset`Class는 UnityEditor 전용입니다.

이 자료형을 사용하면 인스펙터에 노출되며, 빌드시에도 문제가 없습니다.

# 설치 방법 (Git UPM)
#### 2022 LTS or Higher
```` 
https://github.com/NK-Studio/unity-scene-reference.git#UPM
````
#### 2021.3 버전
```
https://github.com/NK-Studio/unity-scene-reference.git#2021.3
```
다음 UPM 주소를 Unity Package Manager에 +버튼을 누르고 추가합니다.

# 사용법

``` C#
using UnityEngine;

public class Demo : MonoBehaviour
{
    public SceneReference TargetScene;
    
    private void Start()
    {
        // Gets the address of the scene.
        Debug.Log(TargetScene.Path);
        
        // Gets the name of the scene.
        Debug.Log(TargetScene.Name);
    }
}
```
