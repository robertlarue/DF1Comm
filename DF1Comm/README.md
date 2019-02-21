[![Build Status](https://dev.azure.com/robertlarue/DF1Comm/_apis/build/status/DF1Comm-.NET%20Desktop-CI?branchName=master)](https://dev.azure.com/robertlarue/DF1Comm/_build/latest?definitionId=3&branchName=master)

## DF1Comm
DF1Comm package is available for download from NuGet http://www.nuget.org/packages/DF1Comm/

Credits to Archie Jacobs of Manufacturing Automation, LLC for the original version of this library. See the original version of this [Allen Bradley DF1 Protocol library](https://sourceforge.net/projects/abdf1/) at SourceForge.

This library no longer relies on the [SerialPort.BytesToRead](https://msdn.microsoft.com/en-us/library/system.io.ports.serialport.bytestoread(v=vs.110).aspx) property which is not available on the Raspberry Pi and other platforms.