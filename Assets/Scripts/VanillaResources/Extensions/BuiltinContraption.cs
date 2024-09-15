using System.Linq;
using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class BuiltinContraption
    {
        public static bool CanEvoke(this Entity contraption)
        {
            if (contraption.Definition is IEvokableContraption evokable)
            {
                return evokable.CanEvoke(contraption);
            }
            return false;
        }
        public static void Evoke(this Entity contraption)
        {
            if (contraption.Definition is IEvokableContraption evokable)
            {
                evokable.Evoke(contraption);
            }
            BuiltinCallbacks.PostContraptionEvoked.Run(contraption);
        }
        public static bool IsEvoked(this Entity contraption)
        {
            return contraption.GetProperty<bool>("Evoked");
        }
        public static void SetEvoked(this Entity contraption, bool value)
        {
            contraption.SetProperty("Evoked", value);
        }
    }
}
