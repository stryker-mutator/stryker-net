const { execSync } = require('child_process');
const readline = require('readline');
const fs = require('fs');
const packagejson = require('./package.json');

const exec = (command) => execSync(command, { stdio: [0, 1, 2] });
const replaceVersionNumber = (path, oldString, newString) => {
    const fileContent = fs.readFileSync(path, { encoding: 'UTF-8' });
    if (!fileContent.includes(oldString)) {
        throw new Error(`The file at ${path} did not contain ${oldString}!`);
    }
    const oldStringStart = fileContent.indexOf(oldString);
    const oldStringEnd = oldStringStart + oldString.length;
    const updatedFileContent = fileContent.substr(0, oldStringStart) + newString + fileContent.substr(oldStringEnd);
    fs.writeFileSync(path, updatedFileContent, { encoding: 'UTF-8' });
};

const removeVersionNumber = (path, oldString, newString) => {
    const fileContent = fs.readFileSync(path, { encoding: 'UTF-8' });
    if (!fileContent.includes(oldString)) {
        throw new Error(`The file at ${path} did not contain ${oldString}!`);
    }
    const oldStringStart = fileContent.indexOf(oldString);
    const oldStringEnd = oldStringStart + oldString.length;
    const updatedFileContent = fileContent.substr(0, oldStringStart) + newString + fileContent.substr(oldStringEnd);
    fs.writeFileSync(path, updatedFileContent, { encoding: 'UTF-8' });
};

const packages = [
    { name: 'stryker', path: './src/Stryker.Core', csproj: './src/Stryker.Core/Stryker.Core/Stryker.Core.csproj' },
    { name: 'dotnet-stryker', path: './src/Stryker.CLI', csproj: './src/Stryker.CLI/Stryker.CLI/Stryker.CLI.csproj' }
];

const oldVersionPrefix = packagejson.versionPrefix;
const oldVersionSuffix = packagejson.versionSuffix;
const oldVersion = oldVersionPrefix + (oldVersionSuffix ?'-':'') + oldVersionSuffix;
console.log(`Current package version is ${oldVersionPrefix}${oldVersionSuffix?'-':''}${oldVersionSuffix}`);
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

rl.question('What should the new package version be? ', (newVersionNumber) => {
    let commitMessageLines = ['Publish', '', ''];
    let versionPrefix = newVersionNumber;
    let versionSuffix = '';

    if (newVersionNumber.indexOf('-')) {
        versionPrefix = newVersionNumber.split('-')[0];
        versionSuffix = newVersionNumber.split('-')[1] ?? '';
    }

    console.log(versionSuffix);

    console.log('Updating package.json');
    replaceVersionNumber('./package.json', `"versionPrefix": "${oldVersionPrefix}",`, `"versionPrefix": "${versionPrefix}",`);
    replaceVersionNumber('./package.json', `"versionSuffix": "${oldVersionSuffix}",`, `"versionSuffix": "${versionSuffix}",`);

    packages.forEach(package => {
        console.log(`Updating version numbers in ${package.csproj}`);
        replaceVersionNumber(package.csproj, `<VersionPrefix>${oldVersionPrefix}</VersionPrefix>`, `<VersionPrefix>${versionPrefix}</VersionPrefix>`);
        replaceVersionNumber(package.csproj, `<VersionSuffix>${oldVersionSuffix}</VersionSuffix>`, `<VersionSuffix>${versionSuffix}</VersionSuffix>`);

        // if (!versionSuffix) {
        //     console.log(`Updating changelog for ${package.name}`);
        //     exec(`npx conventional-changelog-cli -p angular --infile "${package.path}/CHANGELOG.md" --same-file --commit-path ${package.path} --tag-prefix "${package.name}@"`);
        //     commitMessageLines.push(`- ${package.name}@${newVersionNumber}`);
        // }
    });

    // console.log('Updating azure-pipelines.yml');
    // replaceVersionNumber('./azure-pipelines.yml', `VersionBuildNumber: $[counter('${oldVersion}', 1)]`, `VersionBuildNumber: $[counter('${versionPrefix}', 1)]`);
    // replaceVersionNumber('./azure-pipelines.yml', `PackageVersion: '${oldVersion}'`, `PackageVersion: '${versionPrefix}'`);

    // console.log(`Creating commit`);
    // exec('git add .');
    // exec(`git commit ${commitMessageLines.map(entry => `-m "${entry}"`).join(' ')}`);

    // if (!versionSuffix) {
    //     console.log('Tagging commit');
    //     packages.forEach(package => exec(`git tag -a ${package.name}@${newVersionNumber} -m "${package.name}@${newVersionNumber}"`));
    // }

    // console.log(`Pushing commit ${versionSuffix?'':' and tags'}`);
    // exec('git push --follow-tags');
    rl.close();
});

