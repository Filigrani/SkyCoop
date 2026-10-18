using System.Collections.Generic;
using System.IO;
using Il2CppTLD.PDID;
using Il2CppTLD.Gameplay;
using UnityEngine;

// Maps the TLD 2.02 API the mod was written against onto TLD 2.55, so the 50k lines of game logic stay untouched.
// Each member here is a former game member that was removed or moved; keep names identical to the old API.
internal static class Compat
{
    // Gear prefab names: old GearItem.m_GearName == prefab name, now only the GameObject name ("GEAR_X(Clone)", "GEAR_X (1)").
    public static string CleanGearName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        int i = name.IndexOf('(');
        if (i > 0) name = name.Substring(0, i);
        return name.TrimEnd();
    }

    // 2.55 interacts only with objects that have an interaction component; the mod's own interactive objects
    // (other players, dropped gear dummies, fake rock caches...) get a SimpleInteraction, hover text comes from our patch.
    public static void EnsureInteraction(GameObject go)
    {
        if (go.GetComponent<Il2CppTLD.Interactions.BaseInteraction>() != null) return;
        var si = go.AddComponent<Il2CppTLD.Interactions.SimpleInteraction>();
        si.m_DefaultHoverText = new LocalizedString { m_LocalizationID = "" };
        si.HoverText = "";
        si.m_CanInteract = true;
        var dummy = go.GetComponent<SkyCoop.Comps.DroppedGearDummy>();
        if (dummy != null) SetInteractionText(go, dummy.m_LocalizedDisplayName);
    }

    // The HUD may read either the runtime HoverText or the localized default; set both. An unknown localization ID
    // is shown as-is, so the already localized name works as the ID.
    public static void SetInteractionText(GameObject go, string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        var bi = go.GetComponent<Il2CppTLD.Interactions.BaseInteraction>();
        if (bi == null) return;
        bi.m_DefaultHoverText = new LocalizedString { m_LocalizationID = text };
        bi.HoverText = text;
    }

    // Gear prefabs are Addressables now (Resources.Load returns null). Non-gear names must not throw.
    public static GearItem LoadGearPrefab(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        try
        {
            return GearItem.LoadGearItemPrefab(CleanGearName(name));
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    // 2.55 leaves the condition out of the serialized data of cookware (cooking pot, recycled can), so a dropped
    // pot arrived at 0% for the other player. Write it in explicitly.
    public static string SerializeGearForSync(GearItem gi)
    {
        string json = gi.SerializeToString();
        if (json.Contains("m_NormalizedCondition"))
        {
            return json;
        }
        GearItemSaveDataProxy proxy = Utils.DeserializeObject<GearItemSaveDataProxy>(json);
        proxy.m_NormalizedCondition = gi.GetNormalizedCondition();
        proxy.m_CurrentHPProxy = gi.m_CurrentHP;
        return Utils.SerializeObject(proxy);
    }

    // Containers (backpacks, lockers) are prefabs in Addressables, not GearItems.
    static readonly Dictionary<string, GameObject> s_ContainerPrefabs = new Dictionary<string, GameObject>();
    public static GameObject LoadContainerPrefab(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        if (s_ContainerPrefabs.TryGetValue(name, out GameObject cached) && cached != null) return cached;

        GameObject prefab = null;
        foreach (string key in new[] { name, "Assets/Prefabs/Containers/" + name + ".prefab" })
        {
            try
            {
                prefab = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>((Il2CppSystem.Object)key).WaitForCompletion();
            }
            catch (System.Exception) { }
            if (prefab != null) break;
        }
        if (prefab == null)
        {
            GearItem gear = LoadGearPrefab(name);
            if (gear != null) prefab = gear.gameObject;
        }
        if (prefab == null)
        {
            MelonLoader.MelonLogger.Warning("[SkyCoop] Container prefab '" + name + "' not found");
        }
        else
        {
            s_ContainerPrefabs[name] = prefab;
        }
        return prefab;
    }

    // Animal prefabs are Addressables too (WILDLIFE_Wolf, ...), and not GearItems, so LoadGearPrefab can't find them.
    static readonly Dictionary<string, GameObject> s_AnimalPrefabs = new Dictionary<string, GameObject>();
    public static GameObject LoadAnimalPrefab(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        name = CleanGearName(name);
        if (s_AnimalPrefabs.TryGetValue(name, out GameObject cached) && cached != null) return cached;

        GameObject prefab = null;
        try
        {
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>((Il2CppSystem.Object)name);
            prefab = handle.WaitForCompletion();
        }
        catch (System.Exception) { }

        if (prefab == null)
        {
            // Fallback: an animal of this type already present in the scene works as a template.
            foreach (Il2Cpp.BaseAi ai in Resources.FindObjectsOfTypeAll<Il2Cpp.BaseAi>())
            {
                if (ai != null && CleanGearName(ai.gameObject.name) == name) { prefab = ai.gameObject; break; }
            }
        }
        if (prefab != null) s_AnimalPrefabs[name] = prefab;
        return prefab;
    }

    static void WarnMissingGear(string gearName)
        => MelonLoader.MelonLogger.Warning($"[SkyCoop] Gear '{gearName}' not found. GEAR_SC* items need ModComponent + SkyCoop.modcomponent in Mods.");

    extension(GearItem gi)
    {
        public string m_GearName => CleanGearName(gi.name);
        public string m_DisplayName => gi.DisplayName;
        public LocalizedString m_LocalizedDisplayName => gi.GearItemData.m_LocalizedName;
        public float m_MaxHP => gi.GearItemData.MaxHP;
        public float m_DailyHPDecay => gi.GearItemData.DailyHPDecay;
    }

    extension(GearItem gi)
    {
        public float m_WeightKG { get => gi.WeightKG.ToQuantity(1f); set => gi.WeightKG = ItemWeight.FromKilograms(value); }
        // Lives on the shared GearItemData now; only set for the mod's own gear types.
        public void SetDailyHPDecay(float v) => gi.GearItemData.m_DailyHPDecay = v;
        // Save format moved to GearItemSaveDataProxy; the mod sends gear as JSON strings between clients.
        public void Deserialize(string json)
        {
            GearItemSaveDataProxy proxy = Utils.DeserializeObject<GearItemSaveDataProxy>(json);
            float prefabCondition = gi.GetNormalizedCondition();
            gi.Deserialize(proxy, false);
            if (proxy == null) return;
            if (proxy.m_NormalizedCondition > 0f)
            {
                if (gi.GetNormalizedCondition() + 0.001f < proxy.m_NormalizedCondition)
                {
                    gi.SetNormalizedHP(proxy.m_NormalizedCondition, false);
                }
            }
            else if (!json.Contains("m_NormalizedCondition") && !json.Contains("m_CurrentHPProxy") && prefabCondition > 0f)
            {
                // Old drops of cookware (pots, cans) were saved without any condition and loaded as 0%.
                gi.SetNormalizedHP(prefabCondition, false);
            }
        }
        public void Deserialize(GearItemSaveDataProxy proxy) => gi.Deserialize(proxy, false);
    }

    extension(PlayerManager pm)
    {
        // Interaction system became IInteraction based.
        public GameObject m_InteractiveObjectUnderCrosshair => pm.ActiveInteraction?.GetInteractiveObject();
        public Container m_ContainerBeingSearched { get => pm.m_Container; set => pm.m_Container = value; }
        public Harvestable m_HarvestableInProgress { get => pm.m_Harvestable; set => pm.m_Harvestable = value; }

        public GearItem InstantiateItemInPlayerInventory(string gearName) => pm.InstantiateItemInPlayerInventory(gearName, 1);
        // A missing prefab (e.g. SkyCoop.modcomponent not loaded because ModComponent is absent) must not throw:
        // these run inside game start (StartGear) and an exception there aborts loading the region.
        public GearItem InstantiateItemInPlayerInventory(string gearName, int units)
        {
            GearItem prefab = LoadGearPrefab(gearName);
            if (prefab == null) { WarnMissingGear(gearName); return null; }
            return pm.InstantiateItemInPlayerInventory(prefab, units, 1f, PlayerManager.InventoryInstantiateFlags.None);
        }
        public GearItem InstantiateItemAtPlayersFeet(string gearName, int units)
        {
            GearItem prefab = LoadGearPrefab(gearName);
            if (prefab == null) { WarnMissingGear(gearName); return null; }
            return pm.InstantiateItemAtPlayersFeet(prefab, units);
        }

        // Old "pick up with inspect screen" is now the default path of ProcessPickupItemInteraction.
        // Only used for items the mod spawns on the fly (picked-up drops, gifts). ProcessPickupItemInteraction does not
        // finish for such freshly instantiated items in 2.55, so open the inspect screen directly (Take / Put back
        // are then handled by the game). If the inspect screen can't open, give the item directly so it isn't lost.
        public void ProcessInspectablePickupItem(GearItem gi)
        {
            if (gi == null) return;
            gi.gameObject.SetActive(true);
            pm.EnterInspectGearMode(gi);
            if (pm.IsInspectModeActive() && pm.GearItemBeingInspected() == gi)
            {
                MelonLoader.MelonLogger.Msg("[Pickup] " + gi.gameObject.name + " opened in inspect screen");
                return;
            }
            gi.PlayPickUpClip();
            GearItem added = pm.AddItemToPlayerInventory(gi, true, true);
            MelonLoader.MelonLogger.Msg("[Pickup] Inspect screen didn't open for " + gi.gameObject.name + ", added to inventory: " + (added != null));
        }
        public bool ProcessPickupItemInteraction(GearItem gi, bool forceEquip, bool forceEquipImmediate)
            => pm.ProcessPickupItemInteraction(gi, forceEquip, forceEquipImmediate, false);
        // UseBedItem is the inventory action (start placing the bedroll), a placed bed sleeps via its own interaction.
        public void ProcessBedInteraction(Bed bed) => bed.PerformInteraction();
        public void ProcessContainerInteraction(Container c) => c.GetComponent<ContainerInteraction>()?.PerformInteraction();
        public float GetNormalizedSprintReduction() => pm.GetClothingSprintReduction();
    }

    extension(Container c)
    {
        public bool Close() { c.BeginContainerClose(); return true; }
        public void CancelSearch() => c.BeginContainerClose();
        public PlayerControlMode m_RestoreControlMode => PlayerControlMode.Normal;
        public string GetInteractiveActionText() => c.GetComponent<ContainerInteraction>()?.GetInteractiveActionText();
        public bool AddToExistingStackable(string gearName, float normalizedCondition, int units, GearItem gear)
            => c.AddToExistingStackable(gear, normalizedCondition, units);
    }

    extension(Harvestable h)
    {
        public void CancelHarvest() => h.GetComponent<HarvestableInteraction>()?.CancelHold();
    }

    extension(LoadScene door)
    {
        public Lock m_Lock { get => door.Lock; set => door.Lock = value; }
    }

    extension(FireplaceInteraction f)
    {
        public Fire m_Fire => f.Fire;
    }

    extension(LiquidItem l)
    {
        public float m_LiquidLiters { get => l.m_Liquid.ToQuantity(1f); set => l.m_Liquid = ItemLiquidVolume.FromLiters(value); }
    }

    extension(KeroseneLampItem k)
    {
        public float m_MaxFuelLiters => k.m_MaxFuel.ToQuantity(1f);
        public float m_FuelBurnLitersPerHour => k.m_FuelBurnPerHour.ToQuantity(1f);
    }

    extension(Headache h)
    {
        // Headaches are HeadacheData assets now; clone the legacy one with the old per-call parameters.
        public void ApplyHeadache(float durationHours, float pulseFrequencyStart, float pulseFrequencyEnd)
        {
            HeadacheData data = UnityEngine.Object.Instantiate(h.m_LegacyHeadacheData);
            data.m_HeadacheDurationHours = durationHours;
            data.m_HeadachePulseFrequencyStart = pulseFrequencyStart;
            data.m_HeadachePulseFrequencyEnd = pulseFrequencyEnd;
            h.ApplyHeadache(data);
        }
    }

    extension(Panel_SelectExperience.XPModeMenuItem item)
    {
        public ExperienceModeType m_Type => item.m_SandboxConfig?.m_XPMode?.m_ModeType ?? ExperienceModeType.Pilgrim;
    }

    static readonly Dictionary<string, Texture2D> s_TextureCache = new Dictionary<string, Texture2D>();
    static FlyMode s_FlyMode;
    static float s_NextFlyModeLookup;

    extension(Utils)
    {
        public static Texture2D GetCachedTexture(string name) => s_TextureCache.TryGetValue(name, out Texture2D t) && t != null ? t : null;
        public static void CacheTexture(string name, Texture2D tex) => s_TextureCache[name] = tex;
    }

    extension(FlyMode)
    {
        // Static flag is gone; look the component up at most once per second.
        public static bool m_Enabled
        {
            get
            {
                if (s_FlyMode == null && Time.unscaledTime >= s_NextFlyModeLookup)
                {
                    s_NextFlyModeLookup = Time.unscaledTime + 1f;
                    s_FlyMode = UnityEngine.Object.FindObjectOfType<FlyMode>();
                }
                // In 2.55 the FlyMode component exists all the time, so only count it as active
                // while its camera is really detached from the player (otherwise our own doll shows in first person).
                if (s_FlyMode == null || !s_FlyMode.isActiveAndEnabled) return false;
                Camera cam = FlyMode.m_Camera;
                if (cam == null || !cam.isActiveAndEnabled || GameManager.m_PlayerObject == null) return false;
                return Vector3.Distance(cam.transform.position, GameManager.m_PlayerObject.transform.position) > 4f;
            }
        }
    }

    extension(ArrowItem a)
    {
        public void Fire() => a.Fire(1f);
    }

    extension(BowItem b)
    {
        public int GetNumArrowsInInventory() => b.GetNumAllArrowsInInventory();
    }

    extension(PlayerMovement m)
    {
        public float GetModifiedMaxSprintStamina() => m.CurrentMaxStamina;
    }

    extension(Inventory inv)
    {
        public void AddGear(GameObject go) => inv.AddGear(go.GetComponent<GearItem>(), true);
    }

    extension(Panel_BodyHarvest_MenuItem item)
    {
        public float m_HarvestAmount => item.HarvestAmount.ToQuantity(1f);
    }

    extension(RopeAnchorPoint)
    {
        public static Il2CppSystem.Collections.Generic.List<RopeAnchorPoint> m_RopeAnchorPoints => RopeAnchorPoint.s_RopeAnchorPoints;
    }

    extension(ExperienceModeManager emm)
    {
        public static bool IsCurrentEpisodeExperienceMode() => ExperienceModeManager.GetCurrentExperienceModeType() == ExperienceModeType.Story;

        // Modes are GameModeConfig assets now; pick the sandbox config for the requested type.
        public void SetExperienceModeType(ExperienceModeType type)
        {
            var modes = new Il2CppSystem.Collections.Generic.List<GameModeConfig>(emm.GetAvailableGameModes().Cast<Il2CppSystem.Collections.Generic.IEnumerable<GameModeConfig>>());
            GameModeConfig fallback = null;
            foreach (GameModeConfig mode in modes)
            {
                if (mode == null || mode.m_XPMode == null || mode.m_XPMode.m_ModeType != type) continue;
                if (mode.m_SaveSlotType == SaveSlotType.SANDBOX) { emm.SetGameModeConfig(mode); return; }
                fallback ??= mode;
            }
            if (fallback != null) emm.SetGameModeConfig(fallback);
            else MelonLoader.MelonLogger.Warning($"[SkyCoop] No game mode config for {type}");
        }
    }

    extension(ExperienceModeManager)
    {
        public static ExperienceModeType s_CurrentModeType => ExperienceModeManager.GetCurrentExperienceModeType();
    }

    extension(ObjectGuid og)
    {
        public void Set(string guid) => PdidTable.RuntimeAddOrReplace(og, guid);
        public void Generate() => PdidTable.RuntimeAddOrReplace(og, PdidTable.GenerateNewID());
    }

    extension(Utils)
    {
        public static string GetLocalizedRegion(GameRegion region) => RegionIds.LocalizedName((int)region);
        public static string GetLocalizedExperienceMode(ExperienceModeType mode) => mode.ToString();
        public static string GetGuidFromGameObject(GameObject go) => ObjectGuid.MaybeGetGuidFromGameObject(go);
        public static void SetGuidForGameObject(GameObject go, string guid) => ObjectGuid.MaybeAttachObjectGuidAddOrReplace(go, guid);
    }

    // The mod used to put its own keys into the game's save slot. 2.55 rewrote slot storage, so mod keys
    // now live in sidecar files next to the saves: <PersistentDataPath>/SkyCoop/<slot>/<key>.
    static string SidecarPath(string slotName, string key)
    {
        if (string.IsNullOrEmpty(PersistentDataPath.m_Path)) PersistentDataPath.Init();
        return Path.Combine(PersistentDataPath.m_Path, "SkyCoop", slotName, key);
    }

    extension(SaveGameSlots)
    {
        public static bool SaveDataToSlot(SaveSlotType gameMode, Episode episode, uint gameId, string slotName, string key, string data)
        {
            try
            {
                string path = SidecarPath(slotName, key);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, data ?? "");
                return true;
            }
            catch (System.Exception e)
            {
                MelonLoader.MelonLogger.Error($"[SkyCoop] Failed to save {key} for {slotName}: {e}");
                return false;
            }
        }

        public static string LoadDataFromSlot(string slotName, string key)
        {
            if (key == "global")
            {
                // Only used as a stable per-save identity (hashed), not parsed.
                SlotData slot = SaveGameSlots.GetSaveSlotFromName(slotName);
                return slot == null ? null : $"{slot.m_GameId}:{slot.m_InternalName}";
            }
            string path = SidecarPath(slotName, key);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }
    }
}

// Regions became RegionSpecification/SceneSet assets; their asset names match the old GameRegion enum names
// ("LakeRegion", "AirfieldRegion"...). The mod still syncs regions as ints, so map names <-> ints here.
// TFTFT regions without an enum value (e.g. "MiningRegion") get a stable id from their name, identical on every client.
internal static class RegionIds
{
    const int FirstNameHashedId = 1000;

    public static int Of(UnityEngine.Object region)
    {
        if (region == null) return (int)GameRegion.LakeRegion;
        string name = region.name;
        SceneSet scenes = region.TryCast<SceneSet>();
        if (scenes != null)
        {
            foreach (RegionSpecification spec in Resources.FindObjectsOfTypeAll<RegionSpecification>())
            {
                if (spec.Scenes == scenes) { name = spec.name; break; }
            }
        }
        return OfName(name);
    }

    public static int OfName(string name)
    {
        if (System.Enum.TryParse(name, out GameRegion r)) return (int)r;
        // FNV-1a: string.GetHashCode is randomized per process.
        uint h = 2166136261;
        foreach (char c in name) h = (h ^ c) * 16777619;
        return FirstNameHashedId + (int)(h % 1000000);
    }

    public static RegionSpecification Spec(int id)
    {
        foreach (RegionSpecification spec in Resources.FindObjectsOfTypeAll<RegionSpecification>())
        {
            if (OfName(spec.name) == id) return spec;
        }
        return null;
    }

    public static string LocalizedName(int id)
    {
        RegionSpecification spec = Spec(id);
        return spec != null ? spec.ZoneName : ((GameRegion)id).ToString();
    }
}

internal static class RegionManager
{
    public static GameRegion GetCurrentRegion() => (GameRegion)RegionIds.Of(GameManager.TryGetCurrentRegion());
    public static bool GetRegionFromString(string name, out GameRegion region) => System.Enum.TryParse(name, out region);
}

// Old GUID registry, replaced by PdidTable.
internal static class ObjectGuidManager
{
    public static GameObject Lookup(string guid) => string.IsNullOrEmpty(guid) ? null : PdidTable.GetGameObject(guid);
    public static string GenerateNewGuidString() => PdidTable.GenerateNewID();
    public static void UnRegisterGuid(string guid) => PdidTable.RuntimeUnregister(guid);
}

// Diagnostics for "falling through the floor after loading an interior": logs what is actually loaded.
internal static class SceneDiag
{
    public static void Dump(string tag)
    {
        try
        {
            int n = UnityEngine.SceneManagement.SceneManager.sceneCount;
            MelonLoader.MelonLogger.Msg(System.ConsoleColor.Cyan, "[SceneDiag " + tag + "] active=" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " loaded scenes=" + n);
            for (int i = 0; i < n; i++)
            {
                var s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                MelonLoader.MelonLogger.Msg(System.ConsoleColor.Cyan, "[SceneDiag " + tag + "]   " + s.name + " loaded=" + s.isLoaded + " roots=" + (s.isLoaded ? s.rootCount : -1));
            }
            var std = Il2Cpp.GameManager.m_SceneTransitionData;
            if (std != null)
            {
                MelonLoader.MelonLogger.Msg(System.ConsoleColor.Cyan, "[SceneDiag " + tag + "] cur=" + std.m_SceneSaveFilenameCurrent + " next=" + std.m_SceneSaveFilenameNextLoad + " spawn=" + std.m_SpawnPointName + " lastOutdoor=" + std.m_LastOutdoorScene);
            }
            if (Il2Cpp.GameManager.m_PlayerObject != null)
            {
                Vector3 p = Il2Cpp.GameManager.m_PlayerObject.transform.position;
                RaycastHit hit;
                bool ground = Physics.Raycast(p + Vector3.up * 1.5f, Vector3.down, out hit, 50f, ~0, QueryTriggerInteraction.Ignore);
                MelonLoader.MelonLogger.Msg(System.ConsoleColor.Cyan, "[SceneDiag " + tag + "] player=" + p + " ground=" + ground + (ground ? " hit=" + hit.collider.name + " in " + hit.collider.gameObject.scene.name + " at y=" + hit.point.y : ""));
            }
        }
        catch (System.Exception e)
        {
            MelonLoader.MelonLogger.Msg("[SceneDiag] failed: " + e.Message);
        }
    }
}

internal static class StableHashExt
{
    // string.GetHashCode is randomized per process on .NET 6 (it was stable on the old Mono runtime of TLD 2.02).
    // Seeds and keys shared between players or saved to disk must use this instead.
    public static int StableHash(this string s)
    {
        if (s == null) return 0;
        unchecked
        {
            uint h = 2166136261;
            foreach (char c in s) h = (h ^ c) * 16777619;
            return (int)h;
        }
    }
}
