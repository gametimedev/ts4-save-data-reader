using System;
using ProtoBuf;
using s4pi.Package;
using s4pi.Interfaces;
using TS4SaveGame = EA.Sims4.Persistence;

//Steps to import missing stuff
//1. Import  Interface & Package from S4PE
//2. Copy the Proto folder from TS4SimRipper
//3. Install the protobuf-net NuGet package


if (args.Length == 0)
{
    Console.WriteLine("FAILED - No path provided");
    return;
}

//Get savegamepath from args
string savegamepath = args[0];
if (!File.Exists(savegamepath))
{
    Console.WriteLine("FAILED - File not found");
    return;
}

//Options
// -o <outputdir> : output directory (default: new subfolder next to savefile)
// -s <split> : split into type jsons (default: false)
// -f <filter> : filter types (comma separated list of string types, default: all types)

bool split = args.Contains("-s");
string filterarg = null;
int filterindex = Array.IndexOf(args, "-f");
if (filterindex == -1)
{
    filterindex = Array.IndexOf(args, "--filter");
}
if (filterindex != -1 && args.Length > filterindex + 1)
{
    filterarg = args[filterindex + 1];
}
string outputdir = null;
int outputindex = Array.IndexOf(args, "-o");
if (outputindex == -1)
{
    outputindex = Array.IndexOf(args, "--output");
}
if (outputindex != -1 && args.Length > outputindex + 1)
{
    outputdir = args[outputindex + 1];
}
if (outputdir == null)
{
    outputdir = Path.Combine(Path.GetDirectoryName(savegamepath), Path.GetFileNameWithoutExtension(savegamepath));
}
if (!Directory.Exists(outputdir))
{
    Directory.CreateDirectory(outputdir);
}


//Load save
int currentTime = Environment.TickCount;
Package p = (Package)Package.OpenPackage(0, savegamepath, false);
Predicate<IResourceIndexEntry> idel = r => r.ResourceType == 0x0000000D;
IResourceIndexEntry iries = p.Find(idel);
Stream s = p.GetResource(iries);
TS4SaveGame.SaveGameData save = Serializer.Deserialize<TS4SaveGame.SaveGameData>(s);
int loadTime = Environment.TickCount - currentTime;

//Convert and save
if (!split)
{
    //Single file
    string outfile = Path.Combine(outputdir, "savegame-full.json");
    currentTime = Environment.TickCount;
    string json = System.Text.Json.JsonSerializer.Serialize(save, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(outfile, json);
    int saveTime = Environment.TickCount - currentTime;
    Console.WriteLine($"SUCCESS - Saved {outfile} (Load: {loadTime}ms, Save: {saveTime}ms)");
}
