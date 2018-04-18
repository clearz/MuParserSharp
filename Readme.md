MuParserSharp is a direct port of the popular C++ parser written by Ingo Berg into C#. 
The reason for creating this is to prototype ideas in a simple fashion so that
they can later be ported back into the C++ version. This parser runs about 3 times
slower but this is to be expected given the overhead of the .NET platform. The 
base code has kept the C++ formatting style as well as keeping the original variable
names. A copy of this will be kept an a git branch along side a working copy that will 
be brought inline with a more C# friendly naming system. This will allow direct comparison 
between the two versions.