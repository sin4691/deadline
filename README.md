# DeadLine

소리를 내면 쫓아오는 몬스터를 피해 탈출하는 **1인칭 공포 탐사** 게임입니다. 한 달 동안 혼자 기획하고 구현했습니다.

| 장르 | 기간 | 인원 | 엔진 |
|---|---|---|---|
| 1인칭 공포 탐사 | 2026.04 (1개월) | 1인 | Unity 6 · NavMesh · DunGen |

📂 자세한 설명: [포트폴리오 – DeadLine](https://sin4691.github.io/#deadline)

## 핵심 구현

- **소음 반응 몬스터 AI**: 달리기·걷기·앉기에 따라 소음 반경이 달라지고, 반경 안의 몬스터가 추격. 배회·대기·추격·Blocked 4상태 FSM
- **런타임 NavMesh**: 매번 새로 생성되는 던전(DunGen)이 완성된 다음 프레임에 NavMeshSurface를 다시 구워 몬스터 경로 연결
- **인벤토리·상호작용**: Raycast 줍기, 버린 아이템이 벽 너머나 바닥 속에 생기지 않도록 Linecast·Raycast로 위치 보정
- **1인칭 머리 숨기기**: 카메라를 가리는 캐릭터 머리를 Neck 본 스케일로 숨김
- 설정 저장, 점프스케어, 타이머 게임 루프

## 👀 신재윤 작업만 보기

1인 프로젝트입니다. **[`Assets/Scripts`](Assets/Scripts) 폴더의 스크립트는 전부 직접 작성**했습니다. 나머지 폴더는 에셋입니다(Dark Horror UI, DunGen, Subway Modular, polyperfect 등). 아트, 사운드, 던전 생성도 에셋을 사용했습니다.

| 기능 | 파일 |
|---|---|
| 소음 반응 몬스터 AI (FSM) | [`MonsterAI.cs`](Assets/Scripts/MonsterAI.cs) |
| 이동 상태별 소음 반경 | [`PlayerMovement.cs`](Assets/Scripts/PlayerMovement.cs) |
| 런타임 NavMesh | [`DungeonNavMesh.cs`](Assets/Scripts/DungeonNavMesh.cs) |
| 인벤토리·버리기 위치 보정 | [`Inventory.cs`](Assets/Scripts/Inventory.cs) |
| 1인칭 머리 숨기기 | [`HideHead.cs`](Assets/Scripts/HideHead.cs) |
| 설정 저장 | [`SettingsManager.cs`](Assets/Scripts/SettingsManager.cs) |
