Scaling Paths from UnmodifiedPaths Folder:

Before scaling, remove any unnecessary box colliders from the path eg. those on the road side that player will never collide with.

Steps:

1. First drag the path prefab from UnmodifiedPaths folder into scene.

2. Make a duplicate of the path, and remove original path from scene.

Now using the duplicated path:

3. Scale y to 0.5 (Half of its length)

4. Position z to 15 (Move to front of player at origin)

5. Create an empty game object.

6. Change position of that empty game object to (0,0,0)

7. Drag the duplicated path into that empty game object.

8. Rename the 'empty' game object.

9. Drag the named object into the desired folder. It will now be a new prefab.


Note: Do not click apply on duplicate object at any step because it will also modify the original prefab.

