using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Builds mpgear.unity3d: held-item models for other players, taken from the game's own (and TFTFT DLC) gear prefabs.
// Each prefab is named after the gear item; its root is the hand bone frame, so the mod just parents it to hand_r/hand_l.
// The model is fitted onto the matching item already in multiplayerPlayer (same centre, longest axis and length).
// usage: -executeMethod GearModels.Run -mp <multiplayerstuff.unity3d> -game <tld dir> -out <dir> -report <txt>
public static class GearModels
{
    const string Root = "Assets/MPGear";

    // gear name -> (hand, reference child of that hand in multiplayerPlayer)
    static readonly Dictionary<string, (string hand, string reference)> Items = new Dictionary<string, (string, string)>
    {
        { "GEAR_Rifle_Barbs", ("hand_r", "rifleNew") },
        { "GEAR_Rifle_Curators", ("hand_r", "rifleNew") },
        { "GEAR_Rifle_Trader", ("hand_r", "rifleNew") },
        { "GEAR_Rifle_Vaughns", ("hand_r", "rifleNew") },
        { "GEAR_RevolverFancy", ("hand_r", "Revolver") },
        { "GEAR_RevolverGreen", ("hand_r", "Revolver") },
        { "GEAR_RevolverStubNosed", ("hand_r", "Revolver") },
        { "GEAR_Bow_Bushcraft", ("hand_l", "Bow") },
        { "GEAR_Bow_Manufactured", ("hand_l", "Bow") },
        { "GEAR_Bow_Woodwrights", ("hand_l", "Bow") },
        { "GEAR_KeroseneLamp_Spelunkers", ("hand_r", "Lantern") },
        { "GEAR_Knife", ("hand_r", "Torch") },
        { "GEAR_KnifeImprovised", ("hand_r", "Torch") },
        { "GEAR_KnifeScrapMetal", ("hand_r", "Torch") },
        { "GEAR_SurvivalKnife", ("hand_r", "Torch") },
        { "GEAR_CougarClawKnife", ("hand_r", "Torch") },
        { "GEAR_Hatchet", ("hand_r", "Torch") },
        { "GEAR_HatchetImprovised", ("hand_r", "Torch") },
        { "GEAR_FireAxe", ("hand_r", "Torch") },
        { "GEAR_Hammer", ("hand_r", "Torch") },
        { "GEAR_Prybar", ("hand_r", "Torch") },
        { "GEAR_Hacksaw", ("hand_r", "Torch") },
        { "GEAR_Flashlight", ("hand_r", "Torch") },
        { "GEAR_Flashlight_LongLasting", ("hand_r", "Torch") },
        { "GEAR_HandheldShortwave", ("hand_r", "Noicemaker") },
        { "GEAR_Camera", ("hand_r", "Noicemaker") },
    };

    // Per-item correction after the automatic fit (euler degrees in model space), filled from preview renders.
    static readonly Dictionary<string, Vector3> Fix = new Dictionary<string, Vector3>
    {
        { "GEAR_Rifle_Barbs", new Vector3(0, 180, 0) },
        { "GEAR_Rifle_Curators", new Vector3(0, 180, 0) },
        { "GEAR_Rifle_Trader", new Vector3(0, 180, 0) },
        { "GEAR_Rifle_Vaughns", new Vector3(0, 180, 0) },
    };

    static StreamWriter s_Log;
    static readonly Dictionary<Object, Object> s_Copies = new Dictionary<Object, Object>();

    static string Arg(string name)
    {
        string[] a = System.Environment.GetCommandLineArgs();
        int i = System.Array.IndexOf(a, name);
        return i < 0 ? null : a[i + 1];
    }

    public static void Run()
    {
        using (s_Log = new StreamWriter(Arg("-report")))
        {
            try { Build(); }
            catch (System.Exception e) { s_Log.WriteLine("EXCEPTION " + e); }
        }
    }

    static void Build()
    {
        if (AssetDatabase.IsValidFolder(Root)) AssetDatabase.DeleteAsset(Root);
        AssetDatabase.CreateFolder("Assets", "MPGear");

        string game = Arg("-game");
        var bundles = new List<AssetBundle>();
        var prefabPath = new Dictionary<string, (AssetBundle, string)>();
        foreach (string dir in new[] { Path.Combine(game, "tld_Data/StreamingAssets/aa/StandaloneWindows64"), Path.Combine(game, "tld_dlc/TFTFT") })
            foreach (string f in Directory.GetFiles(dir, "*.bundle", SearchOption.AllDirectories))
            {
                AssetBundle b = AssetBundle.LoadFromFile(f);
                if (b == null) { s_Log.WriteLine("cannot load " + f); continue; }
                bundles.Add(b);
                if (b.isStreamedSceneAssetBundle) continue;
                foreach (string n in b.GetAllAssetNames())
                    if (n.EndsWith(".prefab")) prefabPath[Path.GetFileNameWithoutExtension(n).ToLowerInvariant()] = (b, n);
            }
        s_Log.WriteLine($"bundles {bundles.Count}, prefabs {prefabPath.Count}");
        foreach (var kv in prefabPath.Where(p => System.Text.RegularExpressions.Regex.IsMatch(p.Key, "rifle|revolver|knife|hatchet|bow|prybar|flashlight|shortwave|camera|lamp")))
            s_Log.WriteLine($"  prefab {kv.Key} <- {kv.Value.Item2}");

        AssetBundle mp = AssetBundle.LoadFromFile(Arg("-mp"));
        GameObject player = Object.Instantiate(mp.LoadAsset<GameObject>("multiplayerPlayer"));
        s_Anim = player.GetComponent<Animator>();

        var built = new List<string>();
        foreach (var kv in Items)
        {
            if (!prefabPath.TryGetValue(kv.Key.ToLowerInvariant(), out var src)) { s_Log.WriteLine($"MISSING prefab {kv.Key}"); continue; }
            // Instantiate: a prefab asset is never activeInHierarchy.
            GameObject gear = Object.Instantiate(src.Item1.LoadAsset<GameObject>(src.Item2));
            gear.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            Transform hand = FindDeep(player.transform, kv.Value.hand);
            Transform reference = hand.Find(kv.Value.reference);
            Pose(reference.name);
            string path = MakeModel(kv.Key, gear, hand, reference);
            Object.DestroyImmediate(gear);
            if (path != null) built.Add(path);
        }
        AssetDatabase.SaveAssets();

        Previews(player, built);

        Object.DestroyImmediate(player);
        mp.Unload(true);
        foreach (AssetBundle b in bundles) b.Unload(true);

        var build = new AssetBundleBuild { assetBundleName = "mpgear.unity3d", assetNames = built.ToArray() };
        Directory.CreateDirectory(Arg("-out"));
        var manifest = BuildPipeline.BuildAssetBundles(Arg("-out"), new[] { build }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        s_Log.WriteLine(manifest != null ? $"BUILD OK {built.Count} models" : "BUILD FAILED");
    }

    static Animator s_Anim;
    static readonly Dictionary<string, Mesh> s_GpuMeshes = new Dictionary<string, Mesh>();

    // Put the doll in the hold animation the mod plays for this item, so the reference item has its real held scale.
    static void Pose(string reference)
    {
        string hands = reference == "rifleNew" ? "Rifle" : reference == "Revolver" ? "HoldGun" : reference == "Bow" ? "Bow" : reference == "Lantern" ? "HoldLantern" : "No";
        s_Anim.Play("Idle", 0, 0.5f);
        s_Anim.Play(hands, 1, 0.9f);
        for (int i = 0; i < 5; i++) s_Anim.Update(0.1f);
        s_Log.WriteLine($"pose {hands} for {reference}");
    }

    static Transform FindDeep(Transform t, string name)
    {
        if (t.name == name) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform r = FindDeep(t.GetChild(i), name);
            if (r != null) return r;
        }
        return null;
    }

    struct Part { public string name; public Mesh mesh; public Matrix4x4 toWorld; public Material[] mats; }

    // Mesh as it is drawn, in world space. Skinned meshes are baked: their raw vertices are in bind-pose space.
    static Part ToPart(Renderer r)
    {
        if (r is SkinnedMeshRenderer s)
        {
            var baked = new Mesh();
            s.BakeMesh(baked, false);
            baked.name = s.sharedMesh.name;
            baked.RecalculateBounds(); // BakeMesh keeps the bind-pose bounds
            s_Log.WriteLine($"  skinned {r.name} mesh {s.sharedMesh.name} readable {s.sharedMesh.isReadable} verts {s.sharedMesh.vertexCount} baked {baked.vertexCount} bind {s.sharedMesh.bounds.size.ToString("F3")} baked {baked.bounds.size.ToString("F3")} bones {s.bones.Length} root {(s.rootBone ? s.rootBone.name : "-")} lossy {r.transform.lossyScale.ToString("F3")}");
            return new Part { name = r.name, mesh = baked, toWorld = Matrix4x4.TRS(r.transform.position, r.transform.rotation, Vector3.one), mats = r.sharedMaterials };
        }
        return new Part { name = r.name, mesh = r.GetComponent<MeshFilter>().sharedMesh, toWorld = r.transform.localToWorldMatrix, mats = r.sharedMaterials };
    }

    static bool HasMesh(Renderer r) => (r is SkinnedMeshRenderer s && s.sharedMesh != null) || (r is MeshRenderer && r.GetComponent<MeshFilter>()?.sharedMesh != null);

    // Parts that make up the visible model: LOD0 only, skip disabled inspect/broken helpers.
    static List<Part> VisibleParts(GameObject go)
    {
        LODGroup lod = go.GetComponentInChildren<LODGroup>(true);
        IEnumerable<Renderer> rs = lod != null && lod.GetLODs().Length > 0
            ? lod.GetLODs()[0].renderers.Where(r => r != null)
            : go.GetComponentsInChildren<Renderer>(false);
        var parts = rs.Where(r => r.enabled && r.gameObject.activeInHierarchy && HasMesh(r)).Select(ToPart).ToList();
        if (parts.Count == 0)
        {
            s_Log.WriteLine($"  {go.name} renderers: " + string.Join(", ", go.GetComponentsInChildren<Renderer>(true).Select(r => $"{r.name}(go {r.gameObject.activeInHierarchy}, en {r.enabled}, {r.GetType().Name})")));
            // Some prefabs keep the world model on a disabled child; take every renderer that is not a LOD>0 or broken variant.
            parts = go.GetComponentsInChildren<Renderer>(true)
                .Where(r => HasMesh(r) && !System.Text.RegularExpressions.Regex.IsMatch(r.name, "LOD[1-9]|Broken|Ruined|Inspect|Shadow", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                .Select(ToPart).ToList();
        }
        return parts;
    }

    // AABB of parts in the space of `space` (world when null).
    static Bounds BoundsIn(IEnumerable<Part> parts, Transform space)
    {
        bool first = true;
        Bounds b = default;
        foreach (Part p in parts)
        {
            Matrix4x4 toSpace = (space != null ? space.worldToLocalMatrix : Matrix4x4.identity) * p.toWorld;
            Vector3 c = p.mesh.bounds.center, e = p.mesh.bounds.extents;
            for (int i = 0; i < 8; i++)
            {
                Vector3 v = toSpace.MultiplyPoint3x4(c + Vector3.Scale(e, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1)));
                if (first) { b = new Bounds(v, Vector3.zero); first = false; } else b.Encapsulate(v);
            }
        }
        return b;
    }

    static int[] AxesBySize(Vector3 size) => new[] { 0, 1, 2 }.OrderByDescending(i => size[i]).ToArray();

    static Vector3 Axis(int i) => i == 0 ? Vector3.right : i == 1 ? Vector3.up : Vector3.forward;

    static List<Vector3> Points(List<Part> parts, Transform space, int max)
    {
        var pts = new List<Vector3>();
        foreach (Part p in parts)
        {
            if (!p.mesh.isReadable) continue;
            Matrix4x4 m = (space != null ? space.worldToLocalMatrix : Matrix4x4.identity) * p.toWorld;
            foreach (Vector3 v in p.mesh.vertices) pts.Add(m.MultiplyPoint3x4(v));
        }
        int step = Mathf.Max(1, pts.Count / max);
        return pts.Where((_, i) => i % step == 0).ToList();
    }

    static float MeanNearest(List<Vector3> from, List<Vector3> to)
    {
        float sum = 0;
        foreach (Vector3 a in from)
        {
            float best = float.MaxValue;
            foreach (Vector3 b in to) best = Mathf.Min(best, (a - b).sqrMagnitude);
            sum += Mathf.Sqrt(best);
        }
        return sum / from.Count;
    }

    // Try the 24 axis-aligned rotations; keep the one whose vertices lie closest to the reference's (both directions).
    static void FitByVertices(List<Part> refParts, Transform hand, List<Part> gearParts, ref Quaternion rot, ref float scale, ref Vector3 pos)
    {
        List<Vector3> rp = Points(refParts, hand, 400), gp = Points(gearParts, null, 400);
        if (rp.Count == 0 || gp.Count == 0) return;
        Bounds rb = new Bounds(rp[0], Vector3.zero); foreach (var v in rp) rb.Encapsulate(v);
        Bounds gb = new Bounds(gp[0], Vector3.zero); foreach (var v in gp) gb.Encapsulate(v);
        float best = float.MaxValue;
        Vector3[] axes = { Vector3.right, Vector3.up, Vector3.forward, Vector3.left, Vector3.down, Vector3.back };
        foreach (Vector3 fwd in axes)
            foreach (Vector3 up in axes)
            {
                if (Mathf.Abs(Vector3.Dot(fwd, up)) > 0.5f) continue;
                Quaternion q = Quaternion.LookRotation(fwd, up);
                // scale so the rotated gear's AABB diagonal matches the reference's
                Bounds qb = new Bounds(q * gp[0], Vector3.zero); foreach (var v in gp) qb.Encapsulate(q * v);
                float s = rb.size.magnitude / qb.size.magnitude;
                Vector3 t = rb.center - q * gb.center * s;
                var moved = gp.Select(v => q * v * s + t).ToList();
                float score = MeanNearest(moved, rp) + MeanNearest(rp, moved);
                if (score < best) { best = score; rot = q; scale = s; pos = t; }
            }
        s_Log.WriteLine($"  vertex fit score {best:F5} rot {rot.eulerAngles} scale {scale:F4}");
    }

    static List<Part> ReferenceParts(Transform reference)
    {
        // Reference items hidden by zero scale (the animator scales them up while held).
        Vector3 oldScale = reference.localScale;
        if (oldScale == Vector3.zero) reference.localScale = Vector3.one;
        var parts = reference.GetComponentsInChildren<Renderer>(true).Where(r => (r is MeshRenderer || r is SkinnedMeshRenderer) && HasMesh(r)).Select(ToPart).ToList();
        reference.localScale = oldScale;
        return parts;
    }

    static string MakeModel(string name, GameObject gear, Transform hand, Transform reference)
    {
        Bounds rb = BoundsIn(ReferenceParts(reference), hand);
        Bounds rw = BoundsIn(ReferenceParts(reference), null);
        s_Log.WriteLine($"  ref {reference.name} localScale {reference.localScale.ToString("F4")} worldCenter {rw.center.ToString("F3")} worldSize {rw.size.ToString("F3")} hand {hand.position.ToString("F3")}");
        var parts = VisibleParts(gear);
        if (parts.Count == 0) { s_Log.WriteLine($"NO RENDERERS {name}"); return null; }
        // Gear sits at the origin unrotated, so world space = its prefab space with the root scale applied (metres).
        Bounds gb = BoundsIn(parts, null);

        // Rotate the gear's longest/second axes onto the reference's, scale to the reference's length, centre on it.
        int[] ga = AxesBySize(gb.size), ra = AxesBySize(rb.size);
        Quaternion rot = Quaternion.LookRotation(Axis(ra[0]), Axis(ra[1])) * Quaternion.Inverse(Quaternion.LookRotation(Axis(ga[0]), Axis(ga[1])));
        if (Fix.TryGetValue(name, out Vector3 fix)) rot = rot * Quaternion.Euler(fix);
        // Weapons/lanterns match the bundle item they replace; tools held where the torch is keep their real size.
        bool realSize = reference.name == "Torch" || reference.name == "Noicemaker";
        float scale = realSize ? 1f / hand.lossyScale.x : rb.size[ra[0]] / gb.size[ga[0]];
        Vector3 pos = rb.center - rot * (gb.center * scale);
        // Revolvers: the bundle one is taller than long, so axis sizes mislead; fit by vertices instead.
        if (reference.name == "Revolver" && parts.All(p => p.mesh.isReadable)) FitByVertices(ReferenceParts(reference), hand, parts, ref rot, ref scale, ref pos);

        var root = new GameObject(name);
        var model = new GameObject("Model");
        model.transform.SetParent(root.transform, false);
        model.transform.localPosition = pos;
        model.transform.localRotation = rot;
        model.transform.localScale = Vector3.one * scale;
        foreach (Part p in parts)
        {
            // GPU-only game meshes cannot be copied: the part is named "@<mesh>" and the mod assigns the game's mesh at runtime.
            var part = new GameObject(p.mesh.isReadable ? p.name : "@" + p.mesh.name);
            part.transform.SetParent(model.transform, false);
            Matrix4x4 m = p.toWorld;
            part.transform.localPosition = m.GetColumn(3);
            part.transform.localRotation = m.rotation;
            part.transform.localScale = m.lossyScale;
            part.AddComponent<MeshFilter>().sharedMesh = p.mesh.isReadable ? (Mesh)Copy(p.mesh) : null;
            if (!p.mesh.isReadable) s_GpuMeshes[p.mesh.name] = p.mesh;
            part.AddComponent<MeshRenderer>().sharedMaterials = p.mats.Select(mat => (Material)Copy(mat)).ToArray();
        }
        // Lantern keeps the bundle lantern's light; the mod toggles it with the light source state.
        Light lampLight = reference.GetComponentInChildren<Light>(true);
        if (name.Contains("KeroseneLamp") && lampLight != null)
        {
            var l = Object.Instantiate(lampLight.gameObject, root.transform);
            l.name = "Light";
            l.transform.localPosition = hand.InverseTransformPoint(lampLight.transform.position);
            l.SetActive(false);
        }

        string path = $"{Root}/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        s_Log.WriteLine($"model {name} parts {parts.Count} scale {scale:F4} ref {reference.name} refSize {rb.size.ToString("F4")} gearSize {gb.size.ToString("F3")}");
        return path;
    }

    static Object Copy(Object src)
    {
        if (src == null) return null;
        if (s_Copies.TryGetValue(src, out Object done)) return done;
        string safe = string.Join("_", src.name.Split(Path.GetInvalidFileNameChars()));
        Object copy = null;
        if (src is Mesh mesh)
        {
            copy = Object.Instantiate(mesh);
            var check = (Mesh)copy;
            Vector3 before = check.bounds.size;
            check.RecalculateBounds();
            s_Log.WriteLine($"  mesh {mesh.name} readable {mesh.isReadable} attrs [{string.Join(",", mesh.GetVertexAttributes().Select(a => $"{a.attribute}:{a.format}x{a.dimension}@{a.stream}"))}] bounds {before.ToString("F3")} -> recalculated {check.bounds.size.ToString("F3")}");
            AssetDatabase.CreateAsset(copy, AssetDatabase.GenerateUniqueAssetPath($"{Root}/{safe}.asset"));
        }
        else if (src is Material mat)
        {
            // Game shaders can't be rebuilt here; use Standard with the game's albedo/normal maps, like the rest of the player model.
            var m = new Material(Shader.Find("Standard"));
            Texture main = Tex(mat, "_MainTex", "_BaseMap", "_Albedo", "_Diffuse");
            if (main != null) m.SetTexture("_MainTex", (Texture2D)Copy(main));
            Texture nrm = Tex(mat, "_BumpMap", "_NormalMap", "_Normal");
            if (nrm != null)
            {
                m.SetTexture("_BumpMap", (Texture2D)Copy(nrm, true));
                m.EnableKeyword("_NORMALMAP");
            }
            if (mat.HasProperty("_Color")) m.color = mat.color;
            m.SetFloat("_Glossiness", 0.25f);
            s_Log.WriteLine($"  material {mat.name} shader {mat.shader?.name} main {main?.name} normal {nrm?.name} props [{string.Join(",", mat.GetTexturePropertyNames())}]");
            copy = m;
            AssetDatabase.CreateAsset(copy, AssetDatabase.GenerateUniqueAssetPath($"{Root}/{safe}.mat"));
        }
        if (copy != null) copy.name = src.name;
        s_Copies[src] = copy;
        return copy;
    }

    static Texture Tex(Material m, params string[] props)
    {
        foreach (string p in props) if (m.HasProperty(p) && m.GetTexture(p) is Texture2D t) return t;
        return null;
    }

    // Bundle textures are not CPU-readable: read back through a RenderTexture, downscaled (players see these from afar).
    static Object Copy(Texture src, bool normal = false)
    {
        if (s_Copies.TryGetValue(src, out Object done)) return done;
        int w = Mathf.Min(src.width, 512), h = Mathf.Min(src.height, 512);
        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, normal ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
        Graphics.Blit(src, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var readable = new Texture2D(w, h, TextureFormat.RGBA32, true);
        readable.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        readable.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        string path = AssetDatabase.GenerateUniqueAssetPath($"{Root}/{string.Join("_", src.name.Split(Path.GetInvalidFileNameChars()))}.png");
        File.WriteAllBytes(path, readable.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (normal) importer.textureType = TextureImporterType.NormalMap;
        importer.SaveAndReimport();
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        s_Copies[src] = tex;
        return tex;
    }

    // One PNG per model: the bundle's reference item (as is) next to the new model, both in the hand, for checking the fit.
    static void Previews(GameObject player, List<string> built)
    {
        string dir = Arg("-previews");
        if (dir == null) return;
        Directory.CreateDirectory(dir);
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.5f, 0.6f, 0.7f);
        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        lightGo.transform.rotation = Quaternion.Euler(50, -30, 0);
        var rt = new RenderTexture(512, 512, 24);
        cam.targetTexture = rt;
        foreach (string path in built)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var (handName, refName) = Items[prefab.name];
            Pose(refName);
            Transform hand = FindDeep(player.transform, handName);
            Transform reference = hand.Find(refName);
            // Same framing for both passes: the reference item's real (baked) bounds.
            Bounds b = BoundsIn(ReferenceParts(reference), null);
            float d = Mathf.Max(b.extents.magnitude, 0.05f) * 2.5f;
            foreach (int pass in new[] { 0, 1 })
            {
                GameObject inst = null;
                Vector3 old = reference.localScale;
                if (pass == 0)
                {
                    reference.gameObject.SetActive(true);
                    if (old == Vector3.zero) reference.localScale = Vector3.one;
                }
                else
                {
                    inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, hand);
                    foreach (var mf in inst.GetComponentsInChildren<MeshFilter>())
                        if (mf.sharedMesh == null && mf.name.StartsWith("@") && s_GpuMeshes.TryGetValue(mf.name.Substring(1), out Mesh gm)) mf.sharedMesh = gm;
                    foreach (var r in inst.GetComponentsInChildren<MeshRenderer>())
                    {
                        Mesh m = r.GetComponent<MeshFilter>().sharedMesh;
                        s_Log.WriteLine($"  preview {prefab.name}/{r.name} world {r.bounds.size.ToString("F3")} at {r.bounds.center.ToString("F3")} refCenter {b.center.ToString("F3")} mesh {m.name} verts {m.vertexCount} sub {m.subMeshCount} meshSize {m.bounds.size.ToString("F3")}");
                    }
                }
                cam.transform.position = b.center + new Vector3(0.3f, 0.4f, 1f).normalized * d;
                cam.transform.LookAt(b.center);
                cam.nearClipPlane = d * 0.01f;
                cam.farClipPlane = d * 10f;
                cam.Render();
                RenderTexture.active = rt;
                var shot = new Texture2D(512, 512, TextureFormat.RGB24, false);
                shot.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
                RenderTexture.active = null;
                File.WriteAllBytes(Path.Combine(dir, $"{prefab.name}_{(pass == 0 ? "ref" : "new")}.png"), shot.EncodeToPNG());
                if (pass == 0) { reference.gameObject.SetActive(false); reference.localScale = old; }
                else Object.DestroyImmediate(inst);
            }
        }
        Object.DestroyImmediate(camGo);
        Object.DestroyImmediate(lightGo);
    }
}
