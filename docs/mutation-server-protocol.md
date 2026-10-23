---
title: Mutation Server Protocol
sidebar_position: 27
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/mutation-server-protocol.md
---

Stryker.NET can run as a
[Mutation Server Protocol](https://github.com/stryker-mutator/editor-plugins/tree/main/packages/mutation-server-protocol)
server. Editors and other tools can use this server to discover mutants and
start focused mutation test runs.

## Start the server

Use the `serve` command with either the `stdio` or `socket` transport.

```bash
dotnet stryker serve stdio
```

The socket transport requires a port. The address defaults to `localhost`.

```bash
dotnet stryker serve socket --port 9000 --address localhost
```

Pass regular Stryker.NET options after `--`.

```bash
dotnet stryker serve stdio -- --concurrency 1 --project MyProject.csproj
```

For the `stdio` transport, standard output is reserved for JSON-RPC messages.
The server never writes logs or other console output to that stream.

## Protocol support

Stryker.NET supports Mutation Server Protocol version `0.4.0` over JSON-RPC
2.0 with `Content-Length` framing.

The server implements these methods:

- `configure` selects an optional Stryker configuration file.
- `discover` returns mutants for the requested files, directories, or source
  ranges.
- `mutationTest` starts mutation testing for requested files, ranges, or
  previously discovered mutant IDs.

During a mutation test run, each completed mutant is sent in a
`reportMutationTestProgress` notification. The final `mutationTest` response is
an empty files result after all progress notifications have been sent.

File paths may be absolute or relative to the directory where the server was
started. A path ending in `/` selects a directory. Source locations are
1-based, with an inclusive start and exclusive end.
