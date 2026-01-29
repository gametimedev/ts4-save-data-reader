const fs = require('fs');
const path = require('path');

const inputDir = './input_cs';
const outputDir = './output_proto';

const seenMessages = new Set();
const seenEnums = new Set();

function translateFile(content) {
    const lines = [];

    // 1. Extract Enums
    const enumRegex = /public enum (\w+)\s*\{([\s\S]*?)\}/g;
    let enumMatch;
    while ((enumMatch = enumRegex.exec(content)) !== null) {
        const enumName = enumMatch[1];
        if (seenEnums.has(enumName)) continue;
        seenEnums.add(enumName);
        lines.push(`enum ${enumName} {`);
        const usedEnumValues = new Set();
        const memberRegex = /(\w+)\s*=\s*(\d+)/g;
        let m;
        while ((m = memberRegex.exec(enumMatch[2])) !== null) {
            if (!usedEnumValues.has(m[2])) {
                lines.push(`  ${m[1]} = ${m[2]};`);
                usedEnumValues.add(m[2]);
            }
        }
        lines.push('}\n');
    }

    // 2. Extract Messages
    const classRegex = /\[global::ProtoBuf\.ProtoContract\(.*?\)\]\s+public partial class (\w+)/g;
    let classMatch;
    while ((classMatch = classRegex.exec(content)) !== null) {
        const className = classMatch[1];
        if (seenMessages.has(className)) continue;
        seenMessages.add(className);

        lines.push(`message ${className} {`);
        const startIdx = classMatch.index;
        const nextClassMatch = content.slice(startIdx + 1).match(/\[global::ProtoBuf\.ProtoContract/);
        const endIdx = nextClassMatch ? startIdx + nextClassMatch.index : content.length;
        const classScope = content.substring(startIdx, endIdx);

        /**
         * UPDATED REGEX LOGIC:
         * 1. Match [ProtoMember(ID, ...)]
         * 2. Skip any other attributes [xxx]
         * 3. Match 'public'
         * 4. Capture the Type (handles List<...>, arrays, etc)
         * 5. Capture the Name (skipping __pbn__ private fields)
         * 6. Stop only at the semicolon ;
         */
        const memberRegex = /\[global::ProtoBuf\.ProtoMember\((\d+)(?:,.*?DataFormat\s*=\s*global::ProtoBuf\.DataFormat\.(\w+))?.*?\)\]\s+(?:\[.*?\]\s+)*public\s+([\w\.\[\]<>:]+)\s+((?!__pbn__)\w+)[^;]*?;/g;

        let m;
        while ((m = memberRegex.exec(classScope)) !== null) {
            const tag = m[1];
            const format = m[2];
            let type = m[3];
            const name = m[4];

            // 1. Clean the type string thoroughly
            let protoType = type.replace(/global::|System\.Collections\.Generic\.|EA\.Sims4\.|System\./g, '').trim();

            // 2. Handle nested paths (e.g. Actor.Mode -> Mode)
            if (protoType.includes('.')) {
                protoType = protoType.split('.').pop();
            }

            // 3. Determine if it's repeated
            let prefix = "";
            if (protoType === "byte[]" || protoType === "List<byte>") {
                protoType = "bytes";
            } else if (protoType.includes("List<") || protoType.includes("[]")) {
                prefix = "repeated ";
                protoType = protoType.replace(/List<|>/g, '').replace('[]', '');
            }

            // 4. Map C# scalars to Protobuf scalars
            const typeMap = {
                'uint': format === 'FixedSize' ? 'fixed32' : 'uint32',
                'ulong': format === 'FixedSize' ? 'fixed64' : 'uint64',
                'int': format === 'FixedSize' ? 'sfixed32' : 'int32',
                'long': format === 'FixedSize' ? 'sfixed64' : 'int64',
                'bool': 'bool',
                'float': 'float',
                'double': 'double',
                'string': 'string',
                'byte': 'uint32'
            };

            protoType = typeMap[protoType] || protoType;
            lines.push(`  ${prefix}${protoType} ${name} = ${tag};`);
        }
        lines.push('}\n');
    }
    return lines.join('\n');
}

// Execution
if (!fs.existsSync(outputDir)) fs.mkdirSync(outputDir);
const finalProtoLines = ['syntax = "proto3";\npackage EA.Sims4;\n'];
fs.readdirSync(inputDir).filter(f => f.endsWith('.cs')).forEach(file => {
    finalProtoLines.push(translateFile(fs.readFileSync(path.join(inputDir, file), 'utf8')));
});
fs.writeFileSync(path.join(outputDir, "MasterSchema.proto"), finalProtoLines.join('\n'));
console.log("MasterSchema.proto created. Verify SaveGameData now!");