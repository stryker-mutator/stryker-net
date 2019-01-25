const fs = require('fs');

var newVersionNumber = process.argv.slice(2);

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
    { csproj: './integrationtests/TargetProjects/NetStandard2_0/ExampleProject.XUnit/ExampleProject.XUnit.csproj' },
    { csproj: './integrationtests/TargetProjects/NetCore2_1/ExampleProject.XUnit/ExampleProject.XUnit.csproj' }
];

packages.forEach(package => {
    console.log(`Updating DotNetCliToolReference version number in ${package.csproj}`);
    replaceVersionNumber(package.csproj, `<DotNetCliToolReference Include="StrykerMutator.DotNetCoreCli" Version="*" />`, `<DotNetCliToolReference Include="StrykerMutator.DotNetCoreCli" Version="${newVersionNumber}" />`);
});