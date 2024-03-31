# Weather the Storm
A simple(*ish*) mod for Vintage Story that configurably modifies the spawn conditions of drifters blown in by temporal storms, with the goal of allowing players agency in dealing with temporal storms and reducing early-game pressure towards non-gameplay.

This mod only affects drifters spawned by temporal storms. For other surface drifters, check out [Drifters belong underground](https://mods.vintagestory.at/show/mod/9700) by Sotakebk!

---
### Config
The configuration file can be found at `VintagestoryData/ModConfig/weatherthestorm.json`.<br>
The options should be as follows:
```c#
// Should light check be performed at all?
bool AddLightCheck = true;

// The highest block light level a storm drifter can spawn at. Disregards sunlight.
// Only used if AddLightCheck = true
int MaxStormDrifterSpawnLight = 7;

// Should distance check be performed at all?
bool AddDistanceCheck = true;

// The closest a storm drifter can spawn to any player.
// Values greater than 15/(2^0.33) will extend the maximum distance for storm drifter spawns to maintain a reasonable spawnable volume.
// Only used if AddDistanceCheck = true
double MinStormDrifterSpawnDistance = 12;
```
---
#### Feedback and advice appreciated!
This is: 
- Our first Vintage Story mod
- Our first time working with C#
- *And* our first time publishing a project for public use. 

If anything seems out of place, please let us know !