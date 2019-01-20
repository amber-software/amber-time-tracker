using System.ComponentModel;

public enum Platform
{
    [Description("Windows")]
    Windows,

    [Description("MacOS")]
    OSX, 

    [Description("Windows and MacOS")]
    WindowsAndOSX 
}