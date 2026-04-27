# Projectile Buff Testbed

__Репозиторий использует LFS. При скачивании архивом корректный запуск проекта в Unity не гарантируется.__<br>
_Версия Unity 6.3.12f1_

Техническая демонстрация взаимодействия систем: строительство, перемещение и удаление построек, поведения построек, комбинируемые поведения снарядов, модификация свойств снарядов, нанесение урона и уничтожение игровых сущностей.
Используемый стек:
- VContainer
- MessagePipe
- R3
- UniTask
- Addressables
- Input System
- uGUI
- LitMotion

Проект разделен на фичи. Каждая из которых находится в отдельной сборке (assembly definition) и не зависит от других фич. Composition root игровой сессии - GameRunLifetimeScope. Все фичи предоставляют инсталлеры (IInstaller) для регистрации зависимостей. Взаимодействие между фичами осуществляется при помощи MessagePipe (pub/sub, mediator). Core сборка не зависит от UnityEngine, конвертация типов из System.Numerics реализована через UnsafeUtility.As.

## [Билд](https://drive.google.com/file/d/1riMTbqqpjqgX-LhwhzMu7Cjr1nCtDkUQ/view?usp=sharing)

https://github.com/user-attachments/assets/8aa4a074-6367-49ef-b4bd-820e5d755f31

Управление:
- RMB или клик по недоступной ячейке - выход из режима строительства
- Drag-and-drop в переделах зоны строительства переместит постройку в незанятую ячейку, вне пределов - уничтожит ее

Постройки игрока:<br>

<img align="left" width="64" height="64" src="https://github.com/user-attachments/assets/82a28bf8-0613-4d61-ac1d-5b321be4185d"> Добавляет +50% урона от его базового значения
<br clear="left"/>

<img align="left" width="64" height="64" src="https://github.com/user-attachments/assets/de1682de-086a-4340-9315-646626aa3ce0"> Добавляет +1 к количеству пробитий 
<br clear="left"/>

<img align="left" width="64" height="64" src="https://github.com/user-attachments/assets/bea39070-f7be-40da-9ab8-5efdc5d0ac0b"> Создает копию снаряда в начале текущей зоны строительства. Копия имеет на 50% меньший урон и не может быть копирована снова
<br clear="left"/>

Для добавления новой постройки можно дублировать любой префаб в папке Assets/ProjectAssets/Content/Prefabs/Buildings и на компоненте BuildingDescriptionComponent по клику на поле Id добавить и выбрать новый айди. На скриншоте показана кнопка, которая создаст новое значение под PlayerBuilding.

<img width="380" alt="Screenshot 2026-04-15 142516" src="https://github.com/user-attachments/assets/601aab5c-e5b4-4795-9fe9-519dcd44e2fb" />

При запуске игры все префабы из данной папки будут загружены по Addressables label и переданы фабрике, новая постройка отобразится в игре.

Для создания нового ассета конфига снаряда можно дублировать Assets/ProjectAssets/Content/Projectiles/Projectile01/Projectile01.asset, либо создать новый ассет, как показано на скриншоте. В данном проекте такой конфиг используется в префабе 1_TurretBuilding, он залинкован на компоненте BuildingBehaviourDataComponent.

<img height="600" alt="Screenshot 2026-04-27 033648" src="https://github.com/user-attachments/assets/de7134c8-3802-4714-9133-0d75ca5c62bd" />

Поведение проджектайлов задается списком Processors. Например, здесь комбинируется движение по синусоиде с perlin noise.

<img width="380" alt="Screenshot 2026-04-27 033916" src="https://github.com/user-attachments/assets/e0014f2a-95e3-470b-9170-4391a0240473" />

Игровые локации загружаются как префабы. Посмотреть можно в Assets/ProjectAssets/Content/Prefabs/Maps/TestMap.prefab. Объекты BuildingArea задают зоны для строительства. Внутри них можно заранее разместить любые постройки, после загрузки карты они будут доступны в игре.
