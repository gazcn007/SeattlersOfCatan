# SettlersOfCatan3D
An online, multiplayer 3D video game implementation of the popular board game Settlers of Catan made with Unity3D. Includes base game + Seafarers Expansion + Cities & Knights expansion.
* February 2017: Implementation in progress

![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/cover.png)


# Implementation
The game board is procedurally generated. In addition to the original game, SettlersOfCatan3D allows many different map shapes, sizes and orientations that can be determined by the players for a versatile experience.

### Original Map
![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/hexagon1.png)

### Original Orientation with Increased Size
![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/bigmap.png)

### Rectangle Map
![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/rectangleGame.png)

### Triangle Map
![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/triangle1.png)

### Parallelogram Map
![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/parallelogram1.png)


# Creation Timeline
### December 2016
* Started by hexagonal meshes with offset indices and combining them to a single board.

![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/implementation1.png)


* Then added Intersection and Edge classes in proper positions and orientations to hold game pieces.

![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/implementation2.png)


* Textures.

![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/implementation3.png)


* Hexagon Characteristic to yield resources

![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/implementation4.png)

### January 2017
* Game Loop, Settlements and Roads
* Cities, Ships, Resources, Trading

![alt text](https://github.com/nehirakdag/SettlersOfCatan3D/blob/master/Images/regularGame.png)
