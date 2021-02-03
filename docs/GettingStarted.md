# 1 Install

Stryker.NET can both be installed globally and locally.

### Install globally
```
dotnet tool install -g dotnet-stryker
```

### Install in project
Dotnet tools can also be installed on a project level. This requires the following steps:

Create a file called dotnet-tools.json in your project folder, if this is your first local tool.

```
dotnet new tool-manifest
```

Then install stryker without the -g flag by executing the following command in the project folder

```
dotnet tool install dotnet-stryker
```

Check the `dotnet-tools.json` file into source control

Now the rest of your team can install or update stryker with the following command:
`dotnet tool restore`

# 2 Prepare

Make sure the working directory for your console is set to the *unit test* project dir.

# 3 Let's kill some mutants
For most projects no configuration is needed. Simple run stryker and it will find your source project to mutate.

```
dotnet stryker
```

If more configuration is needed follow the instuctions in your console.
