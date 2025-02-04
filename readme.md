
# TS4 Save Data Reader

TS4 Save Data Reader is an npm package to read The Sims 4 save files, specifically the data from its Save Data entry.

## Installation

To install the package, run:

```sh
npm install ts4-save-data-reader
```

## Usage

To use the package, import the `readSaveFileData` function and call it with the path to the save file:

```javascript
const { readSaveFileData } = require('ts4-save-data-reader');

let saveFilePath = "C:\\path\\to\\your\\savefile.save";

readSaveFileData(saveFilePath).then(data => {
    console.log(data);
}).catch(err => console.error(err));
```

## Function

### `readSaveFileData(saveFilePath)`

Reads the data from the Save Data entry of a Sims 4 save file and returns the data as a json string.

- `saveFilePath` (string): The path to the save file.

## Credit

This project is based on two other awesome projects:

- [Sims4Tools](https://github.com/s4ptacle/Sims4Tools) (Reading the package/save file)
- [TS4SimRipper](https://github.com/CmarNYC-Tools/TS4SimRipper) (Proto files to deserialize)

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for details.