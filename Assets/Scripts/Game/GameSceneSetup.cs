using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneSetup : MonoBehaviour
{
    [SerializeField] private Transform playerPos;
    [SerializeField] private PolygonCollider2D cameraConfiner;

    public void SetupCameraBox(int wH, int wW)
    {
        Setup(wH, wW, Vector3Int.zero);
    }

    public void Setup(int worldHeight, int worldWidth, Vector3Int bottomLeftCorner)
    {
        var points = new Vector2[4];
        points[0] = new Vector2(bottomLeftCorner.x - 1, bottomLeftCorner.y - 1);
        points[1] = new Vector2(bottomLeftCorner.x + worldWidth + 1, bottomLeftCorner.y - 1);
        points[2] = new Vector2(bottomLeftCorner.x + worldWidth + 1, bottomLeftCorner.y + worldHeight + 1);
        points[3] = new Vector2(bottomLeftCorner.x - 1, bottomLeftCorner.y + worldHeight + 1);
        cameraConfiner.points = points;
        cameraConfiner.SetPath(0, new List<Vector2>(points));
    }

    public void MovePlayerToSpawnPos(System.Random random, int h, int w, Tilemap boundsTilemap)
    {
        System.Random _r = new System.Random(random.Next());

        bool validSpawn = false;
        Vector3 spawnPos = new Vector3();
        while (!validSpawn)
        {
            validSpawn = true;

            spawnPos = new Vector3(
                _r.Next(h/4, h*3/4),
                _r.Next(w/4, w*3/4),
                0
            );

            for (int i = 0; validSpawn && i < 8; i++)
            {
                Vector3Int checkPos = new Vector3Int(
                    (int) spawnPos.x + Util.SURROUNDING_X[i],
                    (int) spawnPos.y + Util.SURROUNDING_Y[i],
                    0
                );
                validSpawn &= !boundsTilemap.HasTile(checkPos);
            }
        }

        playerPos.position = spawnPos;
    }
}
