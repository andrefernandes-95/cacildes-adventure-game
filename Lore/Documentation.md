### Awake() not firing:

If OnMonoBehaviourEvents are failing due to errors on Colored header script - please check!

### Turning Animations:

Disable Baking Rotations - they can not be applied or else the turning animations will cause the character to stay in loop

### UI Input not working
 Make sure to go to EventSystem and on Actions Asset, make sure it defaults to DefaultInputActions

### Slime and enemies floating in air by climbing on each other
set step offset to 0.1

### bad performance only on editor while playing
Check if its not Version Control window opened, it seems to lag the editor