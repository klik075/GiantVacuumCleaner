# GiantVacuumCleaner

## ✏️ 한 줄 소개
주변 사물을 빨아드려 성장해 더 큰 사물을 빨아드리는 게임

## 📄 개요
- 프로젝트 기간 : 2024.07 ~ 2024.08
- 개발 인원 : 3명(기획1, 아트1, 개발1)
- 장르 : 3D, 캐주얼, 성장
- 다운로드 : x
- 시연 영상 : https://www.youtube.com/shorts/lMh7qx2Snk4

## 🖥️ 기술 스택
- Language : C#
- Engine : Unity 2022.3.62f3
- Tools : Visual Studio 2022, GitHub DeskTop, Notion, Excel

## 🛠️ 주요 기능
- 플레이어 이동
- 물체 랜덤 배치
- 물체 흡입
- 성장
- FBX to Prefab

## 🔧 구현 내용
### 1. Player

폴더 위치 : Assets/@Scripts/Player
- Player : 플레이어의 핵심 데이터 및 게임 플레이 관리
- PlayerController : 플레이어 이동 및 플레이어 조작 처리
- CognitiveRange : 플레이어 주변의 감지 범위 관리

폴더 위치 : Assets/@Scripts/Suck
- SuckController : 진공청소기의 오브젝트 흡입 처리

### 2. Random Spwan

폴더 위치 : Assets/@Scripts/RandomSpawn
- PlayerSpawnContainer : 플레이어가 스폰될 수 있는 위치 관리
- ResourceSpawner : 게임 내 자원 및 물체의 스폰 처리

### 3. FBX to Prefabs

폴더 위치 : Assets/Editor
- FBX to Prefabs : 반복적으로 수행해야 하는 FBX Import 및 Prefab 생성 작업을 에디터에서 자동화

## 📌 참조
사용 에셋 : ActionFit 리소스, QuickOutLine
