# Henrik's Boss Arenas

Аддон для **Calamity Mod**, который в будущем добавит уникальные тематические арены для боёв с подходящими боссами Terraria и Calamity.

## Статус

**Этап 2 — архитектурное исследование.** Выбрана рекомендуемая архитектура, но она ожидает утверждения. Игровые механики, арены, телепортация, порталы и обработка призывов боссов ещё не реализованы.

## Требования

- Terraria 1.4.4 и совместимый стабильный tModLoader;
- .NET SDK, требуемый установленной версией tModLoader (для текущего локального окружения — .NET 8 SDK);
- [Calamity Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2824688072), включённый в tModLoader.

## Зависимости

Calamity является обязательной зависимостью и объявлен в `build.txt` через `modReferences = CalamityMod`. Исходный код и бинарные файлы Calamity не входят в этот репозиторий.

## Локальная сборка

1. Установите tModLoader и Calamity Mod через Steam Workshop.
2. Убедитесь, что `CalamityMod.tmod` доступен в каталоге модов tModLoader.
3. В корне проекта выполните:

   ```powershell
   dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"
   ```

   Если tModLoader установлен в другом месте, передайте фактический путь в свойстве `TModLoaderInstallDir`.

## Запуск в tModLoader

После успешной сборки tModLoader создаёт файл `.tmod` в своём каталоге модов. В меню **Mods** включите Calamity Mod и Henrik's Boss Arenas, затем перезагрузите моды.

## Планируемые возможности

- исследование архитектуры изолированных арен;
- серверно-авторитетное управление сражениями в одиночной игре и мультиплеере;
- расширяемые описания арен;
- тематические арены для подходящих боссов.

Подробности: [архитектура](docs/ARCHITECTURE.md), [исследование архитектуры](docs/ARCHITECTURE_RESEARCH.md), [дорожная карта](docs/ROADMAP.md), [разработка](docs/DEVELOPMENT.md).
