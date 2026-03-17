using UnityEngine;
using System.Collections.Generic;

public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance { get; private set; }

    [Header("All Available Modules")]
    public ModuleData[] allModules;

    private Dictionary<string, ModuleData> moduleLookup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SetInstance();
        InitializeModuleLookup();
    }

    private void SetInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple instances of ModuleManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void InitializeModuleLookup()
    {
        moduleLookup = new Dictionary<string, ModuleData>();
        foreach (var module in allModules)
        {
            if (!moduleLookup.ContainsKey(module.moduleKey))
            {
                moduleLookup.Add(module.moduleKey, module);
            }
            else
            {
                Debug.LogWarning("Duplicate module key found: " + module.moduleKey);
            }
        }
    }

    private List<ModuleData> GetDefaultModules()
    {
        var defaultModules = new List<ModuleData>();
        foreach (var module in allModules)
        {
            if (module.isDefault)
            {
                defaultModules.Add(module);
            }
        }
        return defaultModules;
    }

    public List<ModuleData> GetUnlockedModules()
    {
        var unlockedModules = new List<ModuleData>();
        var unlockedString = PlayerPrefs.GetString("UnlockedModules", "");

        Debug.Log($"Modules unlocked: {unlockedString}");

        if (unlockedString == string.Empty)
        {
            Debug.Log("No modules found, unlocking default modules.");
            var defaultModules = GetDefaultModules();
            foreach (var module in defaultModules)
            {
                UnlockModule(module);
            }
            return defaultModules;
        }

        var unlockedKeys = unlockedString.Split(',');

        foreach (var key in unlockedKeys)
        {
            if (moduleLookup.ContainsKey(key))
            {
                unlockedModules.Add(moduleLookup[key]);
            }
            else
            {
                Debug.LogWarning("Unlocked module key not found in lookup: " + key);
            }
        }

        return unlockedModules;
    }

    public void UnlockModule(string moduleKey)
    {
        Debug.Log($"Unlocking module: {moduleKey}");
        var unlockedKeys = new HashSet<string>(PlayerPrefs.GetString("UnlockedModules", "").Split(','));
        if (!unlockedKeys.Contains(moduleKey))
        {
            unlockedKeys.Add(moduleKey);
            PlayerPrefs.SetString("UnlockedModules", string.Join(",", unlockedKeys));
            PlayerPrefs.Save();
        }
    }

    public void UnlockModule(ModuleData module)
    {
        UnlockModule(module.moduleKey);
    }

    public void UnlockAllModules()
    {
        var allKeys = new List<string>();
        foreach (var module in allModules)
        {
            allKeys.Add(module.moduleKey);
        }
        PlayerPrefs.SetString("UnlockedModules", string.Join(",", allKeys));
        PlayerPrefs.Save();
    }

    public bool IsModuleUnlocked(string moduleKey)
    {
        var unlockedKeys = new HashSet<string>(PlayerPrefs.GetString("UnlockedModules", "").Split(','));
        return unlockedKeys.Contains(moduleKey);
    }

    public bool IsModuleUnlocked(ModuleData module)
    {
        return IsModuleUnlocked(module.moduleKey);
    }
}
