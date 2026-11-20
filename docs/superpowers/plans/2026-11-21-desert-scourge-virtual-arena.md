# Desert Scourge Virtual Arena Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Добавить серверно-авторитетный завершённый бой с Desert Scourge в скрытой временной арене внутри природной пустыни.

**Architecture:** `ArenaSessionSystem` — единственный владелец состояния одной сессии, сохранённых позиций и жизненного цикла арены. `DesertScourgeArenaTrigger` наблюдает штатное появление Calamity NPC и передаёт его системе; генерация временных тайлов, возврат игроков и синхронизация выполняются только сервером.

**Tech Stack:** C#, tModLoader для Terraria 1.4.4, Calamity Mod как обязательная зависимость, PowerShell-проверки, MSBuild.

**Spec:** `docs/superpowers/specs/2026-11-21-virtual-off-map-arena-design.md`

## Global Constraints

- Не обращаться к тайлам, игрокам или NPC за границами `Main.maxTilesX` и `Main.maxTilesY`.
- Не копировать и не заменять исходный код или бинарные файлы Calamity; Desert Medallion остаётся штатным способом призыва.
- Все переходы состояния, изменения тайлов, призыв/перемещение NPC и телепортации выполняет сервер; клиент не является источником истины.
- Поддерживать только одну активную сессию на мир и не начинать её при активном другом боссе.
- При незавершённой сессии после загрузки мира сначала восстановить арену и очистить состояние.
- Не заявлять о поддержке dedicated server, пока не выполнен ручной прогон с двумя игроками.

## Review Focus

- Призыв Desert Scourge вне пустыни: Calamity должен сохранить собственный отказ, а сессия не должна создаваться.
- Повторный призыв во время сессии: не должен перезаписывать снимок или переносить игроков второй раз.
- Игрок, умерший либо отключившийся в бою: его сохранённая позиция не должна потеряться и не должна блокировать очистку.
- Область около границы мира или содержащая неподдерживаемые tile entities: запуск должен безопасно отклоняться без изменения мира.
- Перезагрузка мира в активной сессии: исходные тайлы должны восстановиться до допуска игроков.

---

## Структура файлов

- `Systems/Arenas/ArenaSessionSystem.cs` — серверное состояние одной сессии, сохранение мира, переходы жизненного цикла и возврат участников.
- `Systems/Arenas/ArenaSiteFinder.cs` — поиск допустимого прямоугольника внутри природной пустыни и проверка его содержимого.
- `Systems/Arenas/ArenaTileSnapshot.cs` — снимок и восстановление поддерживаемого минимального набора данных тайла.
- `Systems/Arenas/DesertScourgeArenaBuilder.cs` — серверное построение и сетевое обновление временной арены.
- `Systems/Arenas/DesertScourgeArenaTrigger.cs` — определение целевого NPC Calamity и запуск сессии.
- `Systems/Arenas/ArenaParticipant.cs` — сохранённая исходная позиция и идентификатор участника.
- `tests/verify-arena-source.ps1` — проверка требуемых типов, серверных защит и связей без запуска игры.
- `README.md`, `docs/ROADMAP.md`, `docs/ARCHITECTURE.md` — фактический статус и ограничения прототипа.

### Task 1: Проверяемая модель сессии и выбор участка

**Files:**
- Create: `Systems/Arenas/ArenaParticipant.cs`
- Create: `Systems/Arenas/ArenaSiteFinder.cs`
- Create: `Systems/Arenas/ArenaSessionSystem.cs`
- Create: `tests/verify-arena-source.ps1`

**Interfaces:**
- Produces: `ArenaSiteFinder.TryFindDesertSite(out Rectangle site)`, который возвращает прямоугольник только в пределах мира и только при подтверждённой пустыне.
- Produces: `ArenaSessionSystem.TryStartSession(NPC boss, Player initiator)`, `ArenaSessionSystem.EndSession(ArenaEndReason reason)` и `ArenaSessionSystem.HasActiveSession`.
- Produces: `ArenaParticipant(Guid playerId, Vector2 originalPosition)`.

- [ ] **Step 1: Написать failing-проверку `tests/verify-arena-source.ps1`**

Проверка должна требовать существование трёх типов, `TryFindDesertSite`, `TryStartSession`, `EndSession`, явную защиту `Main.netMode != NetmodeID.MultiplayerClient` и проверку границ мира.

- [ ] **Step 2: Запустить проверку и подтвердить её падение**

Run: `pwsh -File tests/verify-arena-source.ps1`

Expected: FAIL, потому что требуемые исходные файлы и типы отсутствуют.

- [ ] **Step 3: Реализовать минимальные типы и `ArenaSiteFinder.TryFindDesertSite(out Rectangle site)`**

Выбрать фиксированный размер первой арены в одном месте природной пустыни; до записи в мир отклонять прямоугольник с выходом за границы или неподдерживаемыми tile entities. `ArenaSessionSystem` хранит только одну сессию, записывает позиции активных живых игроков перед телепортацией и не создаёт сессию на клиенте.

- [ ] **Step 4: Выполнить исходную проверку**

Run: `pwsh -File tests/verify-arena-source.ps1`

Expected: PASS.

- [ ] **Step 5: Собрать мод**

Run: `dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"`

Expected: `Build succeeded`.

### Task 2: Временные тайлы и восстановление

**Files:**
- Create: `Systems/Arenas/ArenaTileSnapshot.cs`
- Create: `Systems/Arenas/DesertScourgeArenaBuilder.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `tests/verify-arena-source.ps1`

**Interfaces:**
- Consumes: допустимый `Rectangle site` из `ArenaSiteFinder`.
- Produces: `ArenaTileSnapshot.Capture(Rectangle site)`, `ArenaTileSnapshot.Restore()` и `DesertScourgeArenaBuilder.Build(Rectangle site)`.
- Consumes: `ArenaSessionSystem` вызывает `Capture` до `Build` и `Restore` при любом завершении.

- [ ] **Step 1: Расширить failing-проверку порядка операций**

Проверка должна требовать `Capture` до `Build`, `Restore` из `EndSession`, а также синхронизацию изменённого прямоугольника с клиентами.

- [ ] **Step 2: Запустить проверку и подтвердить её падение**

Run: `pwsh -File tests/verify-arena-source.ps1`

Expected: FAIL с отсутствующими snapshot/builder-типами.

- [ ] **Step 3: Реализовать снимок и минимальную арену**

Снимок должен охватывать поддерживаемые свойства тайлов первой версии: наличие/тип/рамки тайла, стена, жидкость, наклон/half-block и провода. Построитель должен создать пол, потолок и боковые барьеры, не изменяя тайлы за `site`, затем передать изменённые tile sections игрокам.

- [ ] **Step 4: Выполнить проверку и сборку**

Run: `pwsh -File tests/verify-arena-source.ps1; dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"`

Expected: обе команды завершаются успешно.

### Task 3: Штатный призыв, перемещение и победа

**Files:**
- Create: `Systems/Arenas/DesertScourgeArenaTrigger.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `HenriksBossArenas.cs`
- Modify: `tests/verify-arena-source.ps1`

**Interfaces:**
- Consumes: `ArenaSessionSystem.TryStartSession(NPC boss, Player initiator)`.
- Produces: обработчик появления NPC, который сопоставляет Calamity `DesertScourgeHead` через публичный `ModContent`/`Mod.Find` API, а не через скопированный тип Calamity.
- Produces: `ArenaSessionSystem.Update()` — завершает сессию при уничтожении связанного босса и вызывает безопасный возврат участников.

- [ ] **Step 1: Расширить failing-проверку запуска и завершения**

Проверка должна требовать, чтобы trigger распознавал только Desert Scourge, запуск был защищён от multiplayer client, босс и активные игроки перемещались сервером, а `Update` обрабатывал победу.

- [ ] **Step 2: Запустить проверку и подтвердить её падение**

Run: `pwsh -File tests/verify-arena-source.ps1`

Expected: FAIL с отсутствующим trigger и обработкой победы.

- [ ] **Step 3: Реализовать trigger и вертикальный сценарий**

Использовать документированный hook появления NPC, который выполняется на сервере. После успешной подготовки: телепортировать записанных участников в безопасные точки арены, перенести связанного босса, выставить `netUpdate`, синхронизировать его и начать отслеживание `whoAmI` с защитой от переиспользования слота. При смерти босса вернуть участников на сохранённые позиции, либо на spawn при недопустимой позиции, затем восстановить арену.

- [ ] **Step 4: Выполнить проверку точки входа, исходную проверку и сборку**

Run: `pwsh -File tests/verify-mod-entrypoint.ps1; pwsh -File tests/verify-arena-source.ps1; dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"`

Expected: все проверки PASS, сборка успешна.

### Task 4: Сохранение, аварийное восстановление и документация

**Files:**
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `tests/verify-arena-source.ps1`
- Modify: `README.md`
- Modify: `docs/ROADMAP.md`
- Modify: `docs/ARCHITECTURE.md`

**Interfaces:**
- Consumes: `ArenaTileSnapshot` и активное серверное состояние сессии.
- Produces: `SaveWorldData(TagCompound tag)`, `LoadWorldData(TagCompound tag)`, `NetSend(BinaryWriter writer)` и `NetReceive(BinaryReader reader)` в `ArenaSessionSystem`.

- [ ] **Step 1: Расширить failing-проверку восстановления**

Проверка должна требовать реализацию `SaveWorldData`/`LoadWorldData` и путь восстановления до допуска игроков после обнаружения незавершённой сессии.

- [ ] **Step 2: Запустить проверку и подтвердить её падение**

Run: `pwsh -File tests/verify-arena-source.ps1`

Expected: FAIL с отсутствующими методами сохранения/восстановления.

- [ ] **Step 3: Реализовать сохранение минимальных данных и аварийную очистку**

Сохранять только данные, необходимые для определения незавершённой сессии и восстановления снимка. Не сохранять клиентское состояние как источник истины. При загрузке мира помечать сессию для серверного восстановления до возможности начать новую.

- [ ] **Step 4: Обновить документацию фактическими возможностями и ограничениями**

Указать Desert Scourge, временную арену в природной пустыне, одну сессию и то, что dedicated server поддержан только после фактического прогона.

- [ ] **Step 5: Выполнить полный набор доступных проверок**

Run: `pwsh -File tests/verify-mod-entrypoint.ps1; pwsh -File tests/verify-arena-source.ps1; dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"; git diff --check`

Expected: все проверки PASS, сборка успешна, `git diff --check` не выводит ошибок.

## Самопроверка плана

- Цель, иллюзия «вне карты», пустынный биом, штатный призыв, серверный авторитет, победа, возврат и восстановление покрыты задачами 1–4.
- Пять рискованных входов из Review Focus имеют владельцев: внепустынный призыв — Task 3; повторный запуск и отключения — Task 1/3; недопустимый участок — Task 1; перезагрузка — Task 4.
- Интерфейсы согласованы: поиск участка → сессия → снимок/построитель → trigger/обновление → сохранение.
- План намеренно не обещает subworld, другие боссы или dedicated-server готовность без ручной проверки.
