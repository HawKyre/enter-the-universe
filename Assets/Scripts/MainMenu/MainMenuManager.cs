using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;
using System;
using Newtonsoft.Json;
using System.Text;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject univDisplayPrefab;
    [SerializeField] private TextMeshProUGUI createWorldName;
    [SerializeField] private SceneTransition sceneTransition;

    private string pUnivDataPath;
    private static int UNIV_DISPLAY_LENGTH = 180;

    public RectTransform univDisplayTransform;
    public RectTransform noContentTransform;

    public List<GameObject> loadedUniverses;

    private void Awake()
    {
        pUnivDataPath = Application.persistentDataPath + "/universes";
        print(pUnivDataPath);

        if (!Directory.Exists(pUnivDataPath))
        {
            Directory.CreateDirectory(pUnivDataPath);
        }

        AssetLoader.LoadGameData();
    }

    public void LoadWorlds()
    {
        // Read the folders are Assets/universes
        string[] univNames = Directory.GetDirectories(pUnivDataPath).Select(x => x.Replace(pUnivDataPath, "").Substring(1)).ToArray();

        if (univNames.Length == 0)
        {
            noContentTransform.gameObject.SetActive(true);
            return;
        }

        for (int i = 0; i < univNames.Length; i++)
        {
            var g = Instantiate(univDisplayPrefab);
            g.transform.SetParent(univDisplayTransform);
            g.GetComponent<UniverseDisplay>().SetupDisplay(univNames[i], new Vector2(0, -UNIV_DISPLAY_LENGTH * i));

            loadedUniverses.Add(g);
        }

        univDisplayTransform.offsetMin = new Vector2(0, -UNIV_DISPLAY_LENGTH * univNames.Length);
        // For each world, load a prefab consisting of the name, a play and a delete
    }

    public void PlayWorld(string name)
    {
        string newUnivPath = pUnivDataPath + "/" + name;
        PlayerPrefs.SetString("univPath", newUnivPath);

        sceneTransition.GoToScene(1);
    }

    public void CreateWorld()
    {
        // File structure setup
        string name = createWorldName.text;
        int i = 1;
        string newUnivPath = pUnivDataPath + "/" + name;
        while (Directory.Exists(newUnivPath))
        {
            newUnivPath = pUnivDataPath + "/" + name + "_" + i;
            i++;
        }

        string univDataPath = newUnivPath + "/univ.dat";
        string playerDataPath = newUnivPath + "/player.dat";

        Directory.CreateDirectory(newUnivPath);
        Directory.CreateDirectory(newUnivPath + "/zones");
        var fs = File.Create(univDataPath);
        fs.Close();

        fs = File.Create(playerDataPath);
        fs.Close();

        // univ.dat setup

        UniverseData uData = new UniverseData();

        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] seeds = new byte[16];
        rng.GetBytes(seeds);

        // Adding seeds
        long seed1 = BitConverter.ToInt64(seeds, 0);
        long seed2 = BitConverter.ToInt64(seeds, 8);

        uData.seed1 = seed1;
        uData.seed2 = seed2;

        // Writing to univ.dat

        string univDat = JsonConvert.SerializeObject(uData);

        File.WriteAllText(univDataPath, univDat, Encoding.UTF8);

        // player.dat setup
        PlayerState playerState = new PlayerState(new Inventory(), Vector3.zero, true);

        string playerDat = JsonConvert.SerializeObject(playerState);

        File.WriteAllText(playerDataPath, playerDat, Encoding.UTF8);

        PlayerPrefs.SetString("univPath", newUnivPath);

        sceneTransition.GoToScene(1);
    }
}
