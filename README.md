

<p align="center">
    <img src="https://github.com/CircuitLord/NotReaper/raw/master/Assets/Images/title.png?sanitize=true"
        height="80">
</p>

<p align="center">
    <a href="https://discord.gg/cakQUt5">
        <img src="https://img.shields.io/discord/545342014913380352?style=plastic&logo=discord"
            alt="chat on Discord"></a>

</br>
</br>
	
</p>

<p align="center">
NotReaper is a custom song and community mapper for the VR rhythm game Audica.  
Unity Version: 2020.1.0b11.3880<br>


<img src="https://github.com/CircuitLord/NotReaper/raw/master/Github/NR1.gif" alt="Main showcase GIF" width=500/>
</p>


<h1 align="center">What can it do?</h1>

<p align ="center">
NotReaper is equipped with many tools and features to help you make Audica maps exactly how you
want in an as short amount of time as possible. Here's some of the highlights.



</p>

* Direct loading, generating, and exporting of Audica files. No more hassle.
* A responsive and user friendly interface. Complete with 100% more animations.
* Drag all the things with Drag Select, your new best friend. Unless you hate productivity, that is.
* Custom-color responsive UI. Want to have teal and gold notes? You got it, chief.
* Mildly entertaining changelogs.



<h1 align="center">More Info</h1>

<h3 align="center">Drag Selecting</h3>

<p align="center">
<img src="https://github.com/CircuitLord/NotReaper/raw/master/Github/NR2.gif" alt="Drag Selecting" width=500/>
</p>
<p align="center">
Make your dreams become a reality with the selector that can be dragged.
</p>


* Drag on the Timeline or the Grid to select a bunch of notes.
* Drag them around to reposition, or drag them on the timeline to change their beat position.
* Flip note colors, position horizontally or vertically. It's flipping awesome.
* When you get bored of holding a bunch of notes hostage, deselect them all with a hotkey.
    * What? I'm not a monster, okay?




<h3 align="center">Customization</h3>

<p align="center">
<img src="https://github.com/CircuitLord/NotReaper/raw/master/Github/NR3.gif" alt="Customization" width=500/>
</p>
<p align="center">
Get bored of staring at the same UI over and over? Make NotReaper more like home.
</p>



* Configure what colors you want to use for the Audica targets, and the UI will follow suite.
* Don't like the background? Me neither. Change it to a picture of kittens for a 20% mapping speed increase.
* Custom keybindings! Want to save with Ctrl+C and add notes with L? Be my guest. (soon)



<h1>How the heck do I use this thing</h1>
https://www.youtube.com/watch?v=4MIahCZRkUg Full tutorial here.

I ask myself that question everyday. For a comprehensive list of the default hotkeys, hit F1 in-game to pull up a neat page with words and animations on it.

For help with using the tool itself, join the [Audica Modding Discord](https://discord.gg/cakQUt5)!


<h1>Credits</h1>
NotReaper is the work of quite a few awesome people.

## NotReaper Team
[CircuitLord](https://github.com/CircuitLord): Lead developer of the project, and video creation guy.

[Jukibom](https://github.com/jukibom): Developer, creator of many of Drag Select's features.

[Mettra](https://github.com/Mettra): Developer, fixer of the undo/redo system nightmare.

[october](https://www.youtube.com/channel/UCEEYUbrnMtNs7XGbOp4Gbdg): Concept artist, graphics designer, and bugtester.

And all the awesome people on Audica Modding Discord who've reported bugs and tested it out.

## History

It all began with [Rolopogo](https://github.com/rolopogo), and the [Audica Editor](https://github.com/rolopogo/AudicaEditor). This was also worked on by [CameronBess](https://github.com/CameronBess) and [Wolf](https://github.com/Wolferacing).

Eventually, all development stopped, and the project went dead. CircuitLord took it up and began to rewrite and rebuild the entire project from what had been there originally. Optimizing, organizing, and adding new features. At some point it had changed so much, that it was decided to rename the project to NotReaper, imitating the main mapping tool used for Audica (Reaper).


# Controls
(These'll get moved later)

Notes:  
`1`: Basic Note  
`2`: Sustain Note  
`3`: Horizontal Note  
`4`: Vertical Note  
`5`: Chain Start  
`6`: Chain Node  
`7`: Melee Note  

`S`: Toggle color

`G`: Switch to grid view  
`N`: Switch to no grid view

Press `V` or hold `Control`: Enter selection mode  
While in selection mode, you can adjust the length of sustain notes  
`Control + X,C, or V` after selecting notes to cut, copy, and paste them.  
`Delete`: Delete selected notes 
`F`: Swap color of selected notes.
`Ctrl + F`: Swap horizontal positon of selected notes.
`Shift + F`: Swap vertical position of selected notes.
`Ctrl + Q-Y`: Change the hitsound of the selected notes.
`Ctrl + D`: Deselect all notes.


`Escape`: Main menu

`Shift + Scroll` while hovering over the big timeline to scale it up and down.  
`Space`: Toggle song playback  
Scroll with the mouse wheel to scrub through the song

Sounds:  
`Q`: Kick  
`W`: Snare  
`E`: Percussion  
`R`: Chain start  
`T`: Chain node  
`Y`: Melee  


# Config File

This is the current config file and all possible values. Note that newer versions do not currently write all keys to the file and just use a default so if your config is out of date please check below for some configurable options you may set.

```

COLOR represents an rgba value as { "r": 0-1, "g": 0-1, "b": 0-1, "a": 0-1 }.

{
    "leftColor": { COLOR },                 // left note color, default { "r": 0.0, "g": 0.5, "b": 1.0, "a": 1.0 }
    "rightColor": { COLOR },                // right note color, default { "r": 1.0, "g": 0.47, "b": 0.14, "a": 1.0 }
    "selectedHighlightColor": { COLOR },    // halo around selected notes, default { "r": 1.0, "g": 1.0, "b": 1.0, "a": 1.0 }
    "mainVol": 0-1,                         // music volume (also in pause menu), default 0.5
    "noteVol": 0-1,                         // note hit volume (also in pause menu), default 0.5
    "sustainVol": 0-1,                      // sustain not hit volume, default 0.5
    "audioDSP": number,                     // DSP buffer size, lower = better latency but may cause crackling, default 480
    "noteHitScale": number,                 // scale transform applied to hit notes, default 0.5 (1.0 for old behaviour)
    "UIFadeDuration": number,               // duration in seconds of certain ui fades, default 1.0
    "useDiscordRichPresence": true / false, // displays current working map in discord, default true
    "showTimeElapsed": true / false,        // displays amount of time using NR in discord, default true
    "bgImagePath": "Path\\To\\Image.png"    // image file to use as a background, default %appdata%\..\locallow\CircuitCubed\NotReaper\BG1.png
}
```
# Donating

Finding NotReaper useful? I'm glad! :) If you really want to, you can donate here:

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=VEEKJW2CSJVZG&currency_code=USD&source=url)


