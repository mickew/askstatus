using System.Reflection;

namespace Askstatus.Sdk;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
