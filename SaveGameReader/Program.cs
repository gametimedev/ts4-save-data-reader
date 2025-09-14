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
// -d <type> : direct output for single type
// -t <tmpfile> : temporary file for direct output (default: none, use stdout)
// Types: save_slot,account,neighborhoods,sims,households,zones,streets,gameplay_data,custom_colors

bool isDirect = args.Contains("-d");
string directType = null;
if (isDirect)
{
    int directIndex = Array.IndexOf(args, "-d");
    if (directIndex == -1)
    {
        directIndex = Array.IndexOf(args, "--direct");
    }
    if (directIndex != -1 && args.Length > directIndex + 1)
    {
        directType = args[directIndex + 1];
    }
    if (directType == null)
    {
        Console.WriteLine("FAILED - No type provided for direct output");
        return;
    }
}

bool split = args.Contains("-s");
string filterarg = "save_slot,account,neighborhoods,sims,households";
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
string outputfile = null;
int outputfileindex = Array.IndexOf(args, "-t");
if (outputfileindex == -1)
{
    outputfileindex = Array.IndexOf(args, "--tmpfile");
}
if (outputfileindex != -1 && args.Length > outputfileindex + 1)
{
    outputfile = args[outputfileindex + 1];
}


//Load save
int currentTime = Environment.TickCount;
Package p = (Package)Package.OpenPackage(0, savegamepath, false);
Predicate<IResourceIndexEntry> idel = r => r.ResourceType == 0x0000000D;
IResourceIndexEntry iries = p.Find(idel);
Stream s = p.GetResource(iries);
TS4SaveGame.SaveGameData save = Serializer.Deserialize<TS4SaveGame.SaveGameData>(s);
int loadTime = Environment.TickCount - currentTime;


//Direct output
if (isDirect && outputfile==null)
{

    if (directType == "save_slot_name")
    {
        Console.WriteLine("SUCCESS:" + save.save_slot.slot_name);
        return;
    }
    else if (directType == "save_slot")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.save_slot, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("SUCCESS:" + json);
        return;
    }
    else if (directType == "account")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.account, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("SUCCESS:" + json);
        return;
    }
    else if (directType == "neighborhoods")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.neighborhoods, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("SUCCESS:" + json);
        return;
    }
    else if (directType == "sims")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.sims, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("SUCCESS:" + json);
        return;
    }
    else if (directType == "households")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.households, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("SUCCESS:" + json);
        return;
    }
    else if (directType == "full")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("SUCCESS:" + json);
        return;
    }
}else if (isDirect)
{

    //Direct to file
    if (directType == "save_slot_name")
    {
        File.WriteAllText(outputfile, save.save_slot.slot_name);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");
        return;
    }
    else if (directType == "save_slot")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.save_slot, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputfile, json);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");
        return;
    }
    else if (directType == "account")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.account, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputfile, json);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");
        return;
    }
    else if (directType == "neighborhoods")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.neighborhoods, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputfile, json);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");
        return;
    }
    else if (directType == "sims")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.sims, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputfile, json);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");
        return;
    }
    else if (directType == "households")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save.households, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputfile, json);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");
        return;
    }
    else if (directType == "full")
    {
        string json = System.Text.Json.JsonSerializer.Serialize(save, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputfile, json);
        Console.WriteLine($"SUCCESS - Saved {outputfile} (Load: {loadTime}ms)");

    }
}

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
else
{
    //Split into types

    string[] filters = filterarg.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    int totalSaved = 0;
    currentTime = Environment.TickCount;
    if (filters.Contains("save_slot"))
    {
        string outfile = Path.Combine(outputdir, "save_slot.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.save_slot, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("account"))
    {
        string outfile = Path.Combine(outputdir, "account.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.account, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("neighborhoods"))
    {
        string outfile = Path.Combine(outputdir, "neighborhoods.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.neighborhoods, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("sims"))
    {
        string outfile = Path.Combine(outputdir, "sims.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.sims, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("households"))
    {
        string outfile = Path.Combine(outputdir, "households.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.households, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("zones"))
    {
        string outfile = Path.Combine(outputdir, "zones.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.zones, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("streets"))
    {
        string outfile = Path.Combine(outputdir, "streets.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.streets, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("gameplay_data"))
    {
        string outfile = Path.Combine(outputdir, "gameplay_data.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.gameplay_data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    if (filters.Contains("custom_colors"))
    {
        string outfile = Path.Combine(outputdir, "custom_colors.json");
        string json = System.Text.Json.JsonSerializer.Serialize(save.custom_colors, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outfile, json);
        totalSaved++;
        Console.WriteLine($"  Saved {outfile}");
    }
    int saveTime = Environment.TickCount - currentTime;
    Console.WriteLine($"SUCCESS - Saved {totalSaved} files (Load: {loadTime}ms, Save: {saveTime}ms)");

}
