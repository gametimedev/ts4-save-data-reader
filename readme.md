

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
- `full`


## Credit

This project is based on two other awesome projects:

- [Sims4Tools](https://github.com/s4ptacle/Sims4Tools) (Reading the package/save file)
- [TS4SimRipper](https://github.com/CmarNYC-Tools/TS4SimRipper) (Proto files to deserialize)


## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for details.