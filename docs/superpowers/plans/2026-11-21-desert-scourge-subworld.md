# Desert Scourge Subworld Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Перенести первый сценарий Desert Scourge в отдельный временный subworld с возвратом и возрождением участников внутри арены.

**Architecture:** `DesertScourgeArenaSubworld` создаёт независимую карту арены через Subworld Library. Серверная сессия хранит состав участников и результат, а trigger переносит участников в subworld вместо изменения основного мира; старый снимок тайлов удаляется.

**Tech Stack:** C#, tModLoader 1.4.4, Calamity Mod, Subworld Library, .NET 8, PowerShell.

**Spec:** `docs/superpowers/specs/2026-11-21-desert-scourge-subworld-design.md`

## Global Constraints

- `SubworldLibrary` — обязательная зависимость; не копировать её исходный код или бинарные файлы.
- Сервер создаёт и завершает сессию; клиент не является источником истины.
- Основной мир не должен получать временные тайлы арены.
- Поддерживается один Desert Scourge-сеанс; другие боссы и параллельные арены не входят в объём.
- Не заявлять dedicated-server совместимость без игрового прогона с двумя игроками.

## Review Focus

- Subworld Library отсутствует или устарела: мод должен отказаться от загрузки с понятной зависимостью, а не упасть при вызове API.
- Calamity не считает арену пустыней: тест должен выявить enraged-поведение до заявления совместимости.
- Игрок умирает в subworld: должен появиться у точки арены, а не в основном мире.
- Победа босса: все участники возвращаются, а основной мир остаётся без изменений.
- Отключение игрока во время перехода: оно не должно блокировать завершение боя или возврат остальных.

---

### Task 1: Обязательная зависимость и независимая карта

**Files:**
- Modify: `build.txt`
- Modify: `HenriksBossArenas.csproj`
- Create: `Systems/Arenas/DesertScourgeArenaSubworld.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`

**Interfaces:**
- Produces: `DesertScourgeArenaSubworld : Subworld` с фиксированными `Width`, `Height`, `Tasks` и `ShouldSave => false`.
- Produces: статические `ArenaSpawnTileX` и `ArenaSpawnTileY` для общих точек входа/возрождения.

- [ ] Написать failing-тест для констант размера и расположения spawn внутри границ subworld.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; убедиться, что он падает из-за отсутствующего типа/значений.
- [ ] Добавить `SubworldLibrary` в `modReferences`, определить subworld с пустынными тайлами и ареной в единственном `GenPass`.
- [ ] Запустить тесты и `dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"`; обе команды должны завершиться успешно.

### Task 2: Серверная сессия и переходы между мирами

**Files:**
- Modify: `Systems/Arenas/ArenaSessionState.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaTrigger.cs`
- Delete: `Systems/Arenas/ArenaTileSnapshot.cs`
- Delete: `Systems/Arenas/ArenaSiteFinder.cs`
- Delete: `Systems/Arenas/DesertScourgeArenaBuilder.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`

**Interfaces:**
- Consumes: `DesertScourgeArenaSubworld` из Task 1.
- Produces: `ArenaSessionSystem.TryStartSession(NPC boss)` — фиксирует участников и переносит их через `SubworldSystem.MovePlayerToSubworld<DesertScourgeArenaSubworld>`.
- Produces: `ArenaSessionSystem.IsParticipant(string playerName)` для респавна и завершения.

- [ ] Написать failing-тест: вторая попытка запуска отклоняется, а `Clear` удаляет участников и привязку к боссу.
- [ ] Запустить тест и убедиться в ожидаемом RED.
- [ ] Перенести trigger на вход в subworld; призыв босса выполняется сервером subworld после подключения участников. Удалить весь код изменений тайлов основного мира.
- [ ] Запустить тесты, проверку entrypoint и сборку.

### Task 3: Возрождение в арене и возврат после победы

**Files:**
- Create: `Players/ArenaParticipantPlayer.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaSubworld.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`

**Interfaces:**
- Consumes: `ArenaSessionSystem.IsParticipant(string playerName)`.
- Produces: `ArenaParticipantPlayer.OnRespawn()` — направляет участника на spawn subworld только при активной сессии.
- Produces: серверный путь победы, возвращающий участников через `SubworldSystem.MovePlayerToMainWorld`.

- [ ] Написать failing-тест для правила: неучастник не считается участником после `Clear`.
- [ ] Запустить тест и увидеть RED.
- [ ] Реализовать `ModPlayer` и серверное завершение сессии; не использовать телепортацию или снимок тайлов основного мира.
- [ ] Запустить тесты, entrypoint-проверку и сборку.

### Task 4: Документация и игровой прогон

**Files:**
- Modify: `README.md`
- Modify: `docs/ARCHITECTURE.md`
- Modify: `docs/ROADMAP.md`
- Modify: `tests/verify-mod-entrypoint.ps1` при необходимости зависимости

- [ ] Обновить документацию: Subworld Library обязательна; основной мир не изменяется; dedicated server ещё не подтверждён.
- [ ] Запустить полный набор: `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`, `pwsh -File tests/verify-mod-entrypoint.ps1`, MSBuild и `git diff --check`.
- [ ] Ручной игровой прогон: одиночная игра — призыв, вход, смерть/возрождение, победа, возврат; затем host-and-play с двумя игроками.
- [ ] Зафиксировать фактические результаты прогона в README/документации, не помечая непроверенный dedicated server как готовый.

## Самопроверка плана

- Отдельная карта, зависимость, переходы, респавн и возврат покрыты задачами 1–3.
- Исходная причина повреждения ландшафта устраняется удалением snapshot/строителя основного мира в Task 2.
- Риски из Review Focus привязаны к Task 1–4; совместимость Desert Scourge и multiplayer подтверждается только ручным прогоном.
