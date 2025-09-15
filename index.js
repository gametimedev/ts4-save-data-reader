const { execFile } = require('child_process');
const path = require('path');
const os = require('os');
const fs = require('fs');

const validTypes = [
    "save_slot_name",
    "save_slot",
    "account",
    "neighborhoods",
    "sims",
    "households",
    "full"
];

function getInfoByType(filepath, type) {
    if (!validTypes.includes(type)) {
        throw new Error('Invalid type provided');
    }

    const tempDir = os.tmpdir();
    const outputFileName = `output_ts4_save_${Date.now()}.json`;
    const outputPath = path.join(tempDir, outputFileName);

    let args = [
        filepath,
        "-t", outputPath,
        "-d", type
    ];

    let executablePath = getExecutablePath();

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

                        //Buffer to json
                        data = JSON.parse(data.toString());

                        resolve(data);
                    });
                });
            });
        });
    });


}

function extractInfoToFolder(filepath, folderPath, filter) {
    if (!fs.existsSync(folderPath)) {
        fs.mkdirSync(folderPath, { recursive: true });
    }
    if (!fs.existsSync(folderPath) || !fs.lstatSync(folderPath).isDirectory()) {
        throw new Error('Provided folder path is not a directory');
    }

    let args = [
        filepath,
        "-o", folderPath,
        "-s"
    ];
    if (filter) {
        args.push("-f", filter);
    }

    let executablePath = getExecutablePath();

    return new Promise((resolve, reject) => {
        execFile(executablePath, args, (error, stdout, stderr) => {
            if (error) {
                console.error(`Error executing file: ${error}`);
                return reject(error);
            }
            resolve(true);
        });
    });

}

function getExecutablePath() {
    let executablePath;
    if (os.platform() === 'win32') {
        executablePath = path.join(__dirname, 'data', 'win', 'SaveGameReader.exe');
    } else if (os.platform() === 'darwin') {
        if (os.arch() === 'arm64') {
            executablePath = path.join(__dirname, 'data', 'osx-arm64', 'SaveGameReader');
        } else {
            executablePath = path.join(__dirname, 'data', 'osx-x64', 'SaveGameReader');
        }
    } else {
        throw new Error('Unsupported OS');
    }
    return executablePath;
}

module.exports = { getInfoByType, extractInfoToFolder };