Collisions
==========

At each frame, we perform:

- Get all the tiles of the current section (not all level tiles)
  - Get the tiles intersecting with the player at the next frame
    - For each of these intersecting tiles, we pick only the nearest from the player (or 2 if they are at equal distance)
      - For each of these nearest tiles, we test if they intersect with the player
        - Get the intersection side (top, left, right, bottom)
        - Correct the position of the player to prevent intersection