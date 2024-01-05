# VsTest internals

## Objective
This document captures knowledge gathered while working on Stryker regarding VsTest related issues. It exists to assist anyone trying to analyse testing problems.
It provides architecture and design description, as well as known limitations and bugs (including github issue number when relevant).

## Test platform architecture in a nutshell
VsTest is architectured to be platform and language neutral. 
From a coder perspective, when one instantiates a session instance (a `IVSTestConsoleWrapper` class), 
this translates to launching a VsTest server process. Its purpose is to carry out test requests
and to send back the results to the client (process). 
When a test session starts, the server process will determine the needed platform
and framework version and starts the adequate test host process that will ultimately perform the tests.
Note that the server process may launch multiple test hosts, such as when the test session covers assemblies 
having different platform/framework constraints, and/or to speed up the process via parallelization.

The server process is then responsible to coordinate all this processes and to forward test results to the client
in a controlled manner.

Ample documentation is available on GiHub: https://github.com/microsoft/vstest

## Zoom on features

### End of test sessions
A client gets the test result via a dedicated handler object (implementing `ITestRunEventsHandler`) that receives notifications
when new test results are available and when the test session ends. 
But the session is also seen as a task, either explicitely when using the async API, or implicitely when using the sync version: the task
is completed/control is returned to the caller when the test session is over.


### Timeout
VsTest supports time out since V 15.5. That is, one can provide a maximum execution time for the test session, 
and VsTest will automatically stops the test session if it is still running when the time is elapsed.

#### Known issues (As of VsTest 17.6,)
- Cancelled test sessions may never end: VsTest may not report the end of a cancelled test session when more than one testhost process was required.
Note that this also applies to session cancelled by the timeout logic. When this happen, the endsession event is not raised, but the task does end.
- Crashed test sessions (StackOverflow, OutOfMemory....) may appear as timeout. Note that the (VsTest) logs will reflect the crash, but the VsTest server
session will still wait for the end session event
- Providing an incorrect TestSettings file will result in nothing happening, without any error report. This happens if the file is corrupted as well as with a negative timeouts
- Some settings will be reported in the logs as not recognized or obsolete, but they are still needed by some test adapters
- The error message `process failed to connect to vstest.console` might indicate that the OS is providing a different version of OpenSSL
  than what VsTest expects. For example, Arch provides OpenSSL3, whereas VsTest expects OpenSSL1. A more
  detailed stack trace might reveal the error `No usable version of libssl was found`. In this case
  you can try the following workaround: 
  - Install `openssl-1.1` (on Arch: `yay -S openssl1.1`). This does not change the default openssl (currently 3.2). It does not require any linking of files!
  - Add the following environment variable before running stryker:
    `CLR_OPENSSL_VERSION_OVERRIDE=1.1`.
  - Example usage: `alias stryker='export CLR_OPENSSL_VERSION_OVERRIDE=1.1 && dotnet stryker'`
  - For details see the following Github issue: https://github.com/stryker-mutator/stryker-net/issues/2799
