## DF1Comm
Credits to Archie over at SourceForge for original version of this [Allen Bradley DF1 Protocol library](https://sourceforge.net/projects/abdf1/).

I modified it so that it does not rely on the [SerialPort.BytesToRead](https://msdn.microsoft.com/en-us/library/system.io.ports.serialport.bytestoread(v=vs.110).aspx) property which is not available on the Raspberry Pi and other platforms.