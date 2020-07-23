using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;
using System;
using Newtonsoft.Json;

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
        // Dictionary<string, string> dataEncoder = new Dictionary<string, string>();

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

        File.WriteAllText(univDataPath, univDat);

        // player.dat setup
        PlayerState playerState = new PlayerState(new Inventory(), Vector2.zero);

        string playerDat = JsonConvert.SerializeObject(playerState);

        File.WriteAllText(playerDataPath, playerDat);

        print(playerDat);

        PlayerPrefs.SetString("univPath", newUnivPath);

        // sceneTransition.GoToScene(1);
    }
}
