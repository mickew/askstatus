using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Askstatus.Sdk;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
