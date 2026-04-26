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
