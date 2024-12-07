using PVZEngine.Damages;

namespace PVZEngine.Armors
{
    public static class EngineArmorExt
    {
        public static ShellDefinition GetShellDefinition(this Armor armor)
        {
            var shellID = armor.GetShellID();
            return armor.Level.Content.GetShellDefinition(shellID);
        }
    }
}
