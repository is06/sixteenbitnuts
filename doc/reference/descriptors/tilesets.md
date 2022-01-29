# Tileset descriptor

## gr (group)

Use a `gr` line to define a tile group. Each group will be shown in the level editor as a single
tool. By defining tile group definitions and tiles, we can draw a lot of tiles and the editor will
know automatically which tile to use in the same group like corners and such.

### Syntax

    gr <name> <autotiling_enabled>

### Arguments

| name               | type   | description                                         |
| ------------------ | ------ | --------------------------------------------------- |
| name               | string | The name of the tile group                          |
| autotiling_enabled | int    | If the auto-tiling should be enabled (1) or not (0) |

### Example

In this example, we define a group called `ground` and the auto-tiling is enabled (1)

    gr ground 1

## gd (group definition)

Use a `gd` line to define a tile type to use in a group for auto-tiling.

### Syntax

    gd <auto_tile_type> <tile_index>

### Arguments

| name           | type   | description                                                          |
| -------------- | ------ | -------------------------------------------------------------------- |
| auto_tile_type | string | the automatic **tile type** to define, see below all possible values |
| tile_index     | int    | which **tile index** the tile type should reference                  |

### Auto tile types

| value             | description                                    |
| ----------------- | ---------------------------------------------- |
| topleft           | Top-left outer corner of the tile group        |
| top               | Top side of the tile group                     |
| topright          | Top-right outer corner of the tile group       |
| left              | Left side of the tile group                    |
| center            | Center of the tile group                       |
| right             | Right side of the tile group                   |
| bottomleft        | Bottom-left outer corner of the tile group     |
| bottom            | Bottom side of the tile group                  |
| bottomright       | Bottom-right outer corner of the tile group    |
| cornertopleft     | Top-left inner corner of the tile group        |
| cornertopright    | Top-right inner corner of the tile group       |
| cornerbottomleft  | Bottom-left inner corner of the tile group     |
| cornerbottomright | Bottom-right inner corner of the tile group    |
| hnarrowleft       | Horizontal narrow left side of the tile group  |
| hnarrowcenter     | Horizontal narrow center of the tile group     |
| hnarrowright      | Horizontal narrow right side of the tile group |
| vnarrowtop        | Vertical narrow left side of the tile group    |
| vnarrowcenter     | Vertical narrow center of the tile group       |
| vnarrowbottom     | Vertical narrow left side of the tile group    |
| single            | Single block of the tile group                 |

### Example

    gd topleft 0
    gd top 1
    gd topright 2

## ti (tile)

Use a `ti` line to define a tile into the current group.

### Syntax

    ti <width> <height> <offset_x> <offset_y> <interaction_type> <layer>

### Arguments

| name             | type | description                                                          |
| ---------------- | ---- | -------------------------------------------------------------------- |
| width            | int  | Width of the tile in pixels                                          |
| height           | int  | Height of the tile in pixels                                         |
| offset_x         | int  | X coord offset on the loaded texture                                 |
| offset_y         | int  | Y coord offset on the loaded texture                                 |
| interaction_type | int  | How the player will interact with the tile (blocked or pass through) |
| layer            | int  | If the tile should display behind or in front of the player          |

### Interaction types

| value | description |
| ----- | ----------- |
| 0     | Obstacle    |
| 1     | Traversable |
| 2     | Platform    |

### Layers

| value | description                         |
| ----- | ----------------------------------- |
| 0     | Foreground (in front of the player) |
| 1     | Background (behind the player)      |

### Example

In this example, the tile line defines a tile of size 16x16 pixels, then an offset zone of 32 (x coord) and 8 (y coord), it's an obstacle (0) and on the foreground layer (0)

    ti 16 16 32 8 0 0
