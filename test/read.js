const protobuf = require("protobufjs");
const fs = require("fs");
const path = require("path");

async function readSims4Data() {
    const protoPath = path.join(__dirname, "output_proto", "MasterSchema.proto");
    const savePath = "C:\\Users\\fabis\\Desktop\\S4MM\\Save\\save.bin";

    try {
        console.log("Loading Schema...");
        const root = await protobuf.load(protoPath);

        // Force resolve all references
        root.resolveAll();

        const SaveGameType = root.lookupType("EA.Sims4.SaveGameData");
        const buffer = fs.readFileSync(savePath);
        const message = SaveGameType.decode(buffer);

        const result = SaveGameType.toObject(message, {
            longs: String,
            enums: String,
            bytes: String,
            defaults: true
        });

        console.log("Decode Successful!");
        fs.writeFileSync("output.json", JSON.stringify(result, null, 2));
        console.log("Saved to output.json");

    } catch (e) {
        console.error("Error:", e.message);
    }
}

readSims4Data();