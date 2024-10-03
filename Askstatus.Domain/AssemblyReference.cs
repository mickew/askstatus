using System.Reflection;

namespace Askstatus.Domain;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
