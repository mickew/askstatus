using System.Reflection;

namespace Askstatus.Web.API;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
