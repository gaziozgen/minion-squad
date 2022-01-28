using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private float levelLength = 100f;

    [SerializeField] private GameObject policeCar = null;
    [SerializeField] private GameObject startTile = null;
    [SerializeField] private GameObject endTile = null;
    [SerializeField] private GameObject tile = null;

    [SerializeField] private Transform tileParent = null;
    [SerializeField] private Transform stuffParent = null;
    
    public void Generate()
    {
        Instantiate(startTile, new Vector3(-3.25f, 0.75f, 0), Quaternion.identity, tileParent);
        int i;
        for (i = 0; i < levelLength; i += 5)
        {
            Instantiate(tile, new Vector3(-3.25f, 0.75f, i), Quaternion.identity, tileParent);
        }
        Instantiate(endTile, new Vector3(-3.25f, 0.75f, i), Quaternion.identity, tileParent);
    }

    public void ResetLevel()
    {
        for (int i = tileParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tileParent.GetChild(i).gameObject);
        }

        for (int i = stuffParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(stuffParent.GetChild(i).gameObject);
        }
    }
}
