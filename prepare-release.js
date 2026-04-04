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

    console.log('Updating package.json');
    replaceVersionNumber('./package.json', `"version": "${packagejson.version}",`, `"version": "${newVersionNumber}",`);
    replaceVersionNumber('./package.json', `"versionPrefix": "${oldVersionPrefix}",`, `"versionPrefix": "${versionPrefix}",`);
    replaceVersionNumber('./package.json', `"versionSuffix": "${oldVersionSuffix}",`, `"versionSuffix": "${versionSuffix}",`);

    packages.forEach(pckg => {
        console.log(`Updating version numbers in ${pckg.csproj}`);
        replaceVersionNumber(pckg.csproj, `<VersionPrefix>${oldVersionPrefix}</VersionPrefix>`, `<VersionPrefix>${versionPrefix}</VersionPrefix>`);
        replaceVersionNumber(pckg.csproj, `<VersionSuffix>${oldVersionSuffix}</VersionSuffix>`, `<VersionSuffix>${versionSuffix}</VersionSuffix>`);
    });

    let releaseNotes = '';
    if (!versionSuffix) {
        console.log(`Updating changelog`);
        commitMessageLines.push(`- dotnet-stryker@${newVersionNumber}`);
        releaseNotes = execSync(`npx conventional-changelog-cli -p angular --tag-prefix "dotnet-stryker@"`, { encoding: 'utf8' }).trim();
        const changelogPath = './CHANGELOG.md';
        const changelog = fs.readFileSync(changelogPath, { encoding: 'UTF-8' });
        const marker = '<!-- changelog -->';
        if (!changelog.includes(marker)) {
            throw new Error(`${changelogPath} is missing the '${marker}' insertion marker`);
        }
        fs.writeFileSync(changelogPath, changelog.replace(marker, `${marker}\n\n${releaseNotes}`), { encoding: 'UTF-8' });
    }

    console.log('Updating azure-pipelines.yml');
    replaceVersionNumber('./azure-pipelines.yml', `VersionBuildNumber: $[counter('${oldVersion}', 1)]`, `VersionBuildNumber: $[counter('${versionPrefix}', 1)]`);
    replaceVersionNumber('./azure-pipelines.yml', `PackageVersion: '${oldVersion}'`, `PackageVersion: '${versionPrefix}'`);

    if (!versionSuffix) {
        console.log('Tagging commit');
        const tmpTagFile = '.release-notes.md';
        fs.writeFileSync(tmpTagFile, releaseNotes);
        exec(`git tag -a dotnet-stryker@${newVersionNumber} -F ${tmpTagFile}`);
        fs.unlinkSync(tmpTagFile);
    }

    console.log(`Creating commit`);
    exec('git add .');
    exec(`git commit ${commitMessageLines.map(entry => `-m "${entry}"`).join(' ')}`);

    console.log(`Pushing commit ${versionSuffix?'':' and tags'}`);
    exec('git push --follow-tags');
    if (!versionSuffix) {
        try {
            const execSync = require('node:child_process').execSync;
            execSync(`gh release create dotnet-stryker@${newVersionNumber} --title "dotnet-stryker@${newVersionNumber}" --notes-from-tag`);
            console.log(`Created GitHub release for dotnet-stryker@${newVersionNumber}`);
        } catch (e) {
            console.warn('Failed to create GitHub release:', e.message);
        }
    }
    rl.close();
});

