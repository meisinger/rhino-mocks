Rhino Mocks
===========

**Rhino Mocks** is a dynamic mock object framework for the .Net platform.

How To Build
------------

Rhino Mocks is built using [psake](https://github.com/psake/psake). It is important to understand that the standard Visual Studio build process will not work due to missing AssemblyInfo.cs files. This is to ensure the build number is generated correctly.

In order to build this project, execute the [psake](https://github.com/psake/psake) build script from a Powershell console. This can be done using the following command from the root project directory:

*.\psake.ps1*

Two tasks are available beyond the default task: *Compile* and *Test*

You make need to allow script execution by running the following command as administrator:

*Set-ExecutionPolicy unrestricted*

**The build script assumes *git.exe* is in your PATH**

Mailing List
------------

Visit [Rhino Mocks](https://groups.google.com/group/rhinomocks "Rhino Mocks Google Group") for questions or issues with the framework. This leaves the GitHub repository "Issues" tab to focus on code and compilation issues.