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

## 코드 위치

[`Assets/Scripts`](Assets/Scripts) — `MonsterAI.cs`, `PlayerMovement.cs`, `DungeonNavMesh.cs`, `Inventory.cs`, `HideHead.cs`

아트·사운드·던전 생성은 에셋(Dark Horror UI, DunGen, Subway Modular 등)을 사용했습니다.
