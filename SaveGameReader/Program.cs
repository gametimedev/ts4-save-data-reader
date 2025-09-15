using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ProtoBuf;
using s4pi.Interfaces;
using s4pi.Package;
using TS4SaveGame = EA.Sims4.Persistence;

/// <summary>
/// A utility to parse and extract data from a Sims 4 save game file.
/// </summary>
public class SavegameExtractor
{
    private const uint SavegameResourceType = 0x0000000D;

    public static void Main(string[] args)
    {
        try
        {
            var options = ParseArguments(args);
            if (options == null) return;

            Console.WriteLine($"Processing savegame: {options.SavegamePath}");
            var saveGameData = LoadSaveGame(options.SavegamePath);

            if (options.IsDirect)
            {
                HandleDirectOutput(saveGameData, options);
            }
            else
            {
                HandleFileOutput(saveGameData, options);
            }

            Console.WriteLine("Operation completed successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"FAILED - File not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Parses the command-line arguments and returns an Options object.
    /// </summary>
    private static Options ParseArguments(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("FAILED - No path provided.");
            return null;
        }

        var options = new Options();
        options.SavegamePath = args[0];

        // Parse command-line options using a utility method
        var argMap = ParseArgMap(args.Skip(1).ToArray());
        options.IsDirect = argMap.ContainsKey("-d");
        options.Split = argMap.ContainsKey("-s");

        options.Filter = argMap.TryGetValue("-f", out var filterValue)
            ? filterValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : new[] { "save_slot", "account", "neighborhoods", "sims", "households", "zones", "streets", "gameplay_data", "custom_colors" };

        options.OutputDir = argMap.TryGetValue("-o", out var outputDirValue)
            ? outputDirValue
            : Path.Combine(Path.GetDirectoryName(options.SavegamePath), Path.GetFileNameWithoutExtension(options.SavegamePath));

        options.DirectType = argMap.TryGetValue("-d", out var directTypeValue) ? directTypeValue : null;
        options.DirectFile = argMap.TryGetValue("-t", out var directFileValue) ? directFileValue : null;

        // Check for required direct output type
        if (options.IsDirect && string.IsNullOrEmpty(options.DirectType))
        {
            Console.WriteLine("FAILED - No type provided for direct output.");
            return null;
        }

        return options;
    }

    /// <summary>
    /// Loads and deserializes the save game data.
    /// </summary>
    private static TS4SaveGame.SaveGameData LoadSaveGame(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The specified save game file was not found.", path);
        }

        Console.WriteLine("Loading save game...");
        var startTime = Environment.TickCount;

        using (var package = (Package)Package.OpenPackage(0, path, false))
        {
            var saveResourceEntry = package.Find(r => r.ResourceType == SavegameResourceType);
            if (saveResourceEntry == null)
            {
                throw new InvalidOperationException("Could not find the save game resource within the package.");
            }

            using (var stream = package.GetResource(saveResourceEntry))
            {
                var save = Serializer.Deserialize<TS4SaveGame.SaveGameData>(stream);
                var loadTime = Environment.TickCount - startTime;
                Console.WriteLine($"Load successful. Time taken: {loadTime}ms");
                return save;
            }
        }
    }

    /// <summary>
    /// Handles the output for a single, direct type to console or a file.
    /// </summary>
    private static void HandleDirectOutput(TS4SaveGame.SaveGameData save, Options options)
    {
        string outputData = GetSerializedDataByType(save, options.DirectType);
        if (outputData == null)
        {
            Console.WriteLine($"FAILED - Unknown type: {options.DirectType}");
            return;
        }

        if (string.IsNullOrEmpty(options.DirectFile))
        {
            Console.WriteLine($"SUCCESS:{outputData}");
        }
        else
        {
            File.WriteAllText(options.DirectFile, outputData);
            Console.WriteLine($"SUCCESS - Saved direct output to {options.DirectFile}");
        }
    }

    /// <summary>
    /// Handles saving output to one or more files.
    /// </summary>
    private static void HandleFileOutput(TS4SaveGame.SaveGameData save, Options options)
    {
        if (!Directory.Exists(options.OutputDir))
        {
            Directory.CreateDirectory(options.OutputDir);
        }

        var startTime = Environment.TickCount;
        int filesSaved = 0;

        if (options.Split)
        {
            var typesToSave = GetTypesToSave(save, options.Filter);
            foreach (var type in typesToSave)
            {
                var outfile = Path.Combine(options.OutputDir, $"{type.Key}.json");
                File.WriteAllText(outfile, type.Value);
                Console.WriteLine($"  Saved {outfile}");
                filesSaved++;
            }
        }
        else
        {
            var outfile = Path.Combine(options.OutputDir, "savegame-full.json");
            var json = JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true , NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString });
            File.WriteAllText(outfile, json);
            filesSaved++;
        }

        var saveTime = Environment.TickCount - startTime;
        Console.WriteLine($"SUCCESS - Saved {filesSaved} file(s). Time taken: {saveTime}ms");
    }

    /// <summary>
    /// Gets the serialized data for a specific type.
    /// </summary>
    private static string GetSerializedDataByType(TS4SaveGame.SaveGameData save, string typeName)
    {
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true, NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString };

        return typeName switch
        {
            "save_slot_name" => "{\"name\":\""+save.save_slot.slot_name+"\"}",
            "save_slot" => JsonSerializer.Serialize(save.save_slot, jsonOptions),
            "account" => JsonSerializer.Serialize(save.account, jsonOptions),
            "neighborhoods" => JsonSerializer.Serialize(save.neighborhoods, jsonOptions),
            "sims" => JsonSerializer.Serialize(save.sims, jsonOptions),
            "households" => JsonSerializer.Serialize(save.households, jsonOptions),
            "zones" => JsonSerializer.Serialize(save.zones, jsonOptions),
            "streets" => JsonSerializer.Serialize(save.streets, jsonOptions),
            "gameplay_data" => JsonSerializer.Serialize(save.gameplay_data, jsonOptions),
            "custom_colors" => JsonSerializer.Serialize(save.custom_colors, jsonOptions),
            "full" => JsonSerializer.Serialize(save, jsonOptions),
            _ => null
        };
    }

    /// <summary>
    /// Gets a dictionary of type names and their serialized JSON strings.
    /// </summary>
    private static Dictionary<string, string> GetTypesToSave(TS4SaveGame.SaveGameData save, string[] filters)
    {
        var types = new Dictionary<string, object>
        {
            ["save_slot"] = save.save_slot,
            ["account"] = save.account,
            ["neighborhoods"] = save.neighborhoods,
            ["sims"] = save.sims,
            ["households"] = save.households,
            ["zones"] = save.zones,
            ["streets"] = save.streets,
            ["gameplay_data"] = save.gameplay_data,
            ["custom_colors"] = save.custom_colors,
        };

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true , NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString };
        var filteredData = new Dictionary<string, string>();

        foreach (var filter in filters)
        {
            if (types.TryGetValue(filter, out var data))
            {
                filteredData[filter] = JsonSerializer.Serialize(data, jsonOptions);
            }
        }

        return filteredData;
    }

    /// <summary>
    /// Utility method to parse command-line arguments into a dictionary.
    /// </summary>
    private static Dictionary<string, string> ParseArgMap(string[] args)
    {
        var argMap = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-"))
            {
                var key = args[i].ToLower();
                var value = (i + 1 < args.Length && !args[i + 1].StartsWith("-")) ? args[++i] : string.Empty;
                argMap[key] = value;
            }
        }
        return argMap;
    }

    /// <summary>
    /// A helper class to hold parsed command-line options.
    /// </summary>
    private class Options
    {
        public string SavegamePath { get; set; }
        public bool IsDirect { get; set; }
        public bool Split { get; set; }
        public string[] Filter { get; set; }
        public string OutputDir { get; set; }
        public string DirectType { get; set; }
        public string DirectFile { get; set; }
    }
}