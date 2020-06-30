
Unity Motion Capture v1.1.0

1. Add 'Unity Motion Capture' script to Scene
2. Fill inspector of 'Unity Motion Capture'
    2-1. Capture Game Object : The game object what you want capture
    2-2. Animation Save Path : The path of save directory
    2-3. Animation Name : The name of animation clip that will be create
    2-4. Sample Rate : Sampling count of animation clip at 1 second. 
    2-5. Use Quarternion : Use quarternion rotation (default = euler)
    2-6. Ignore Root Transform : Not capture root motion
    2-7. Ignore Scale : Not capture scale
3. Start scene
4. Click 'capture' button to start capture object
5. Click 'recoding' button to stop capture and save animation file

Large animation clips can cause Unity to freeze momentarily.



GUI 구성 목표

트렌스폼 리스트 만들기
카메라 프리뷰를 구성, 에니메이션을 넣어서 컨트롤 할 수 있다.

다 끝나면 로고 넣기