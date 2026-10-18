using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

// Rebuilds the Unity 2018 "auto-mapped/scitems.unity3d" of SkyCoop.modcomponent as an Addressables
// bundle + JSON catalog, the only format ModComponent 7 accepts. Addresses = original asset names.
// usage: -executeMethod Repack.Run -bundle <scitems.unity3d> -report <txt>
public static class Repack
{
    const string Root = "Assets/SC";
    static readonly Dictionary<Object, Object> s_Copies = new Dictionary<Object, Object>();
    static StreamWriter s_Log;

    static string Arg(string name)
    {
        string[] a = System.Environment.GetCommandLineArgs();
        return a[System.Array.IndexOf(a, name) + 1];
    }

    public static void Run()
    {
        using (s_Log = new StreamWriter(Arg("-report")))
        {
            if (Directory.Exists(Root)) AssetDatabase.DeleteAsset(Root);
            Directory.CreateDirectory(Root);
            AssetBundle bundle = AssetBundle.LoadFromFile(Arg("-bundle"));
            var topLevel = new List<Object>();
            foreach (Object o in bundle.LoadAllAssets())
            {
                Object copy = o is GameObject go ? SavePrefab(go) : CopyAsset(o);
                if (copy != null) topLevel.Add(copy);
            }
            bundle.Unload(true);
            AssetDatabase.SaveAssets();
            BuildAddressables(topLevel);
        }
    }

    static GameObject SavePrefab(GameObject src)
    {
        GameObject inst = Object.Instantiate(src);
        inst.name = src.name;
        foreach (MeshFilter mf in inst.GetComponentsInChildren<MeshFilter>(true)) mf.sharedMesh = (Mesh)CopyAsset(mf.sharedMesh);
        foreach (SkinnedMeshRenderer smr in inst.GetComponentsInChildren<SkinnedMeshRenderer>(true)) smr.sharedMesh = (Mesh)CopyAsset(smr.sharedMesh);
        foreach (MeshCollider mc in inst.GetComponentsInChildren<MeshCollider>(true)) mc.sharedMesh = (Mesh)CopyAsset(mc.sharedMesh);
        foreach (Renderer r in inst.GetComponentsInChildren<Renderer>(true))
            r.sharedMaterials = r.sharedMaterials.Select(m => (Material)CopyAsset(m)).ToArray();
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(inst, $"{Root}/{src.name}.prefab");
        Object.DestroyImmediate(inst);
        s_Log.WriteLine($"prefab {src.name}");
        return prefab;
    }

    static Object CopyAsset(Object src)
    {
        if (src == null) return null;
        if (s_Copies.TryGetValue(src, out Object done)) return done;
        Object copy = null;
        string safe = string.Join("_", src.name.Split(Path.GetInvalidFileNameChars()));
        switch (src)
        {
            case Mesh mesh:
                copy = Object.Instantiate(mesh);
                AssetDatabase.CreateAsset(copy, Unique($"{Root}/{safe}.asset"));
                break;
            case Material mat:
                var m = new Material(mat);
                // Shaders embedded in the 2018 bundle die with it; rebind to this editor's shader of the same name.
                Shader shader = mat.shader != null ? Shader.Find(mat.shader.name) : null;
                s_Log.WriteLine($"material {mat.name} shader {(mat.shader != null ? mat.shader.name : "null")} -> {(shader != null ? "ok" : "MISSING")}");
                if (shader != null) m.shader = shader;
                foreach (string prop in mat.GetTexturePropertyNames())
                    if (mat.GetTexture(prop) is Texture2D t) m.SetTexture(prop, (Texture2D)CopyAsset(t));
                copy = m;
                AssetDatabase.CreateAsset(copy, Unique($"{Root}/{safe}.mat"));
                break;
            case Texture2D tex:
                copy = SaveTexture(tex, safe);
                break;
            case TextAsset text:
                string path = Unique($"{Root}/{safe}.txt");
                File.WriteAllBytes(path, text.bytes);
                AssetDatabase.ImportAsset(path);
                copy = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                break;
            default:
                s_Log.WriteLine($"SKIP {src.GetType().Name} {src.name}");
                break;
        }
        if (copy != null) copy.name = src.name;
        s_Copies[src] = copy;
        return copy;
    }

    // Bundle textures are not CPU-readable: read them back through a RenderTexture.
    static Texture2D SaveTexture(Texture2D src, string safe)
    {
        var rt = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Graphics.Blit(src, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var readable = new Texture2D(src.width, src.height, TextureFormat.RGBA32, src.mipmapCount > 1);
        readable.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
        readable.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        string path = Unique($"{Root}/{safe}.png");
        File.WriteAllBytes(path, readable.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = src.mipmapCount > 1;
        importer.wrapMode = src.wrapMode;
        importer.filterMode = src.filterMode;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static string Unique(string path) => AssetDatabase.GenerateUniqueAssetPath(path);

    static void BuildAddressables(List<Object> assets)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        settings.EnableJsonCatalog = true;
        EditorUtility.SetDirty(settings);
        AddressableAssetGroup group = settings.FindGroup("SkyCoop")
            ?? settings.CreateGroup("SkyCoop", false, false, true, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
        var schema = group.GetSchema<BundledAssetGroupSchema>();
        schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
        schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
        schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
        schema.IncludeInBuild = true;
        foreach (Object a in assets)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(a));
            AddressableAssetEntry e = settings.CreateOrMoveEntry(guid, group, false, false);
            e.address = a.name;
            s_Log.WriteLine($"address {a.name}");
        }
        AssetDatabase.SaveAssets();
        AddressableAssetSettings.BuildPlayerContent(out var result);
        s_Log.WriteLine(string.IsNullOrEmpty(result.Error) ? $"BUILD OK {result.OutputPath}" : $"BUILD ERROR {result.Error}");
    }
}
