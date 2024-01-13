# Scene Reference
유니티에서 씬 에셋을 참조하려는 의도가 있다면, SceneReference를 사용하세요.

유니티에서 제공하는 `Scene`Class는 인스펙터에 노출되지 않으며, `SceneAsset`Class는 UnityEditor 전용입니다.

이 자료형을 사용하면 인스펙터에 노출되며, 빌드시에도 문제가 없습니다.

# 설치 방법 (Git UPM)
#### 2021.3 LTS or Higher
```` 
https://github.com/NK-Studio/SceneReference.git#UPM
````
다음 UPM 주소를 Unity Package Manager에 +버튼을 누르고 추가합니다.

# 사용법

``` C#
using UnityEngine;

public class Demo : MonoBehaviour
{
    public SceneReference TargetScene;
    
    private void Start()
    {
        // 타겟 씬의 경로를 반환합니다.
        Debug.Log(TargetScene.Path);
        Debug.Log(TargetScene.ToString);
        
        // 타겟 씬의 이름을 반환합니다.
        Debug.Log(TargetScene.Name);

        // 타겟 씬이 비어있는지 체크합니다.
        if (TargetScene.IsEmpty)
        {
            Debug.Log("Scene is empty");
        }
    }
}
```
