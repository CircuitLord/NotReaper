# Edica version Cameron1.2

## How to use
* If you want to change the resolution of the application hold the shift key while launching Edica
* Convert your music to OGG-VORBIS `.ogg` before use
* Save will give you a .audica file. This file can be used to save progress on your map without exporting it.


*If you have already started charting a track*, your beat markers may not line up with the grid properly - find the correct offset value to match the timeline beat grid to the music, and add an entry to `song.desc` that reads `"offset": x` where x is the offset you found

## Importing work from other editors:
1. Open edica
2. Click "New edica project" and select the `.ogg` file for the song
3. Select difficulty (bottom right)
4. Click "Import .cues" and select the cues file for your current difficulty
5. Repeat steps 3&4 for all of your difficulties
6. You can now edit the project as normal. When saving, the project will be saved in the `.edica` format automatically.

## Upgrading projects from Edica version "Cameron1.1.1" to "Cameron1.2"
1. Open Edica and load your `.edica` file
2. When loaded it will automatically set you previous work to "expert" difficulty. If the save was meant for expert mode you can stop at this step.
3. To change difficulties click "Export". This will create an expert.cues file at the specified location.
4. Switch to desired difficulty for this save and press "import .cues"
5. Find the "expert.cues" file you just exported and import it into this difficulty.
6. Now the .edica save will keep track of multiple difficulties automatically.



