using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Extensions
{
    public static class MVZ2Entity
    {
        public static void PlaySound(this Entity entity, NamespaceID soundID, float pitch = 1)
        {
            entity.Level.PlaySound(soundID, entity.Pos, pitch);
        }
        public static int GetSortingLayer(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.SORTING_LAYER);
        }
        public static void SetSortingLayer(this Entity entity, int layer)
        {
            entity.SetProperty(BuiltinEntityProps.SORTING_LAYER, layer);
        }
        public static int GetSortingOrder(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinEntityProps.SORTING_ORDER);
        }
        public static void SetSortingOrder(this Entity entity, int layer)
        {
            entity.SetProperty(BuiltinEntityProps.SORTING_ORDER, layer);
        }
    }
}
