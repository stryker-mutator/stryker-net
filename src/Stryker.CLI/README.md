# Stryker.NET Command Line Interface
The Stryker CLI is currently the only implemented way to use Stryker.NET. 

## Getting started
To install Stryker.NET on your *test project* add the following lines to the root of your `.csproj` file.

``` XML
<ItemGroup>
    <DotNetCliToolReference Include="StrykerMutator.DotNetCoreCli" Version="*" />
    <PackageReference Include="StrykerMutator.DotNetCoreCli" Version="*" />
</ItemGroup>
```

After adding the references, install the packages by executing `dotnet restore` inside the project folder.

## Usage
To kick off stryker, execute the following command inside the test project folder:

`dotnet stryker`

## Configuration
While Stryker.NET wants to be a non configuration needed tool, some settings are configurable.

#### Specify your project to mutate
When Stryker finds two or more project references inside your test project, it needs to know what project should be mutated. Pass the name of this project using:

`dotnet stryker --project SomeProjectName.csproj`

The name will be matched to the full path. You won't have to pass the full path, as long as the name is unique for the found references.

#### Specify extra timeout time
Some mutations can create endless loops inside your code. To detect and stop these loops, Stryker generates timeouts after some time. Using this parameter you can increase or decrease the time before a timeout will be thrown.

`dotnet stryker --timeoutMS 5000`

Defaults to `2000`

#### Logging to console

`dotnet stryker --logConsole <loglevel>`

All available loglevels are:
* error (default)
* warning
* info
* debug
* trace

#### Logging to a file

`dotnet stryker --logFile`
