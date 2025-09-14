const { execFile } = require('child_process');
const path = require('path');
const os = require('os');
const fs = require('fs');

const validTypes = [
    "save_slot",
    "account",
    "neighborhoods",
    "sims",
    "households",
    "full"
];

function readSaveFileData(saveFilePath) {
    if (!saveFilePath) {
        throw new Error('No save file path provided');
    } else if (!fs.existsSync(saveFilePath)) {
        throw new Error('Save file does not exist');
    }

    let executablePath;
    if (os.platform() === 'win32') {
        executablePath = path.join(__dirname, 'data', 'SaveToJson.exe');
    } else if (os.platform() === 'darwin') {
        executablePath = path.join(__dirname, 'data', 'SaveToJson');
    } else {
        throw new Error('Unsupported OS');
    }
    const tempDir = os.tmpdir();
    const outputFileName = `output_ts4_save_${Date.now()}.json`;
    const outputPath = path.join(tempDir, outputFileName);
    const args = [saveFilePath, outputPath];
    return new Promise((resolve, reject) => {
        execFile(executablePath, args, (error, stdout, stderr) => {
            if (error) {
                console.error(`Error executing file: ${error}`);
                return reject(error);
            }
            fs.access(outputPath, fs.constants.F_OK, (err) => {
                if (err) {
                    return reject(new Error('Output file was not created'));
                }
                fs.readFile(outputPath, (err, data) => {
                    if (err) {
                        return reject(err);
                    }
                    fs.unlink(outputPath, (err) => {
                        if (err) {
                            return reject(err);
                        }
                        resolve(data);
                    });
                });
            });
        });
    });
}

function getInfoByType(type) {
    if (!validTypes.includes(type)) {
        throw new Error('Invalid type provided');
    }

}

function getSaveFileName(saveFilePath) {

}

module.exports = { readSaveFileData };