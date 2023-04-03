using MelonLoader;
using System.Reflection;

[assembly: AssemblyTitle("SkyCoop")]
[assembly: AssemblyCopyright($"Created by Filigrani")]

[assembly: AssemblyVersion(SkyCoop.MyMod.BuildInfo.Version)]
[assembly: AssemblyFileVersion(SkyCoop.MyMod.BuildInfo.Version)]
[assembly: MelonInfo(typeof(SkyCoop.MyMod), SkyCoop.MyMod.BuildInfo.Name, SkyCoop.MyMod.BuildInfo.Version, SkyCoop.MyMod.BuildInfo.Author)]

[assembly: MelonGame("Hinterland", "TheLongDark")]