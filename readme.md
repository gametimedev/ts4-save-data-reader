# TS4 Save Data Reader

TS4 Save Data Reader is an npm package to extract and read data from The Sims 4 save files using a native executable. It supports extracting specific data types and outputting structured data to folders or JSON.


## Installation

Install via npm:

```sh
npm install ts4-save-data-reader
```


## Usage

Import the functions:

```javascript
const { getInfoByType, extractInfoToFolder } = require('ts4-save-data-reader');
```

### Extract specific info as JSON

```javascript
const saveFile = "C:\\path\\to\\your\\savefile.save";
getInfoByType(saveFile, "account")
    .then(data => {
        console.log(data);
    })
    .catch(err => console.error(err));
```

### Extract all info to a folder

```javascript
const saveFile = "C:\\path\\to\\your\\savefile.save";
const outFolder = "C:\\path\\to\\output";
extractInfoToFolder(saveFile, outFolder)
    .then(success => {
        console.log("Extraction successful:", success);
    })
    .catch(err => console.error(err));
```


## API

### `getInfoByType(filepath, type)`
Extracts a specific type of data from the save file and returns it as a JSON object.

- `filepath` (string): Path to the Sims 4 save file.
- `type` (string): Type of info to extract (see below).

### `extractInfoToFolder(filepath, folderPath, filter?)`
Extracts all (or filtered) info from the save file and writes it to the specified folder.

- `filepath` (string): Path to the Sims 4 save file.
- `folderPath` (string): Output folder path.
- `filter` (optional string): Filter for specific data.

### Available types for `getInfoByType`

- `save_slot_name`
- `save_slot`
- `account`
- `neighborhoods`
- `sims`
- `households`
- `zones`
- `streets`
- `gameplay_data`
- `custom_colors`
- `full`


## Command-Line Utility

The `SavegameExtractor` class in the `Program.cs` file provides a command-line utility to process Sims 4 save files. Below are the available options:

### Usage

```sh
SaveGameReader <savegame_path> [options]
```

### Options

- `-d <type>`: Direct output of a specific type to the console or a file (requires `-t` for file output).
- `-t <file>`: Specifies the file path for direct output.
- `-s`: Splits the output into multiple files based on data types.
- `-o <folder>`: Specifies the output folder for extracted data.
- `-f <filters>`: Filters the data types to extract (comma-separated).

### Example Commands

#### Direct Output to Console
```sh
SaveGameReader "C:\\path\\to\\savefile.save" -d account
```

#### Direct Output to File
```sh
SaveGameReader "C:\\path\\to\\savefile.save" -d account -t "C:\\path\\to\\output\\account.json"
```

#### Extract All Data to Folder
```sh
SaveGameReader "C:\\path\\to\\savefile.save" -o "C:\\path\\to\\output"
```

#### Extract Filtered Data to Folder
```sh
SaveGameReader "C:\\path\\to\\savefile.save" -o "C:\\path\\to\\output" -f sims,households
```


## Credit

This project is based on two other awesome projects:

- [Sims4Tools](https://github.com/s4ptacle/Sims4Tools) (Reading the package/save file)
- [TS4SimRipper](https://github.com/CmarNYC-Tools/TS4SimRipper) (Proto files to deserialize)


## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for details.