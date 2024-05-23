using System;
using System.Collections.Generic;
using System.Linq;

namespace PVZEngine
{
    public partial class Game
    {
        public void SetHeldItem(int type, int id, int priority, bool noCancel = false)
        {
            OnHeldItemChanged?.Invoke(type, id, priority, noCancel);
        }
        public void ResetHeldItem()
        {
            OnHeldItemReset?.Invoke();
        }
        public event Action<int, int, int, bool> OnHeldItemChanged;
        public event Action OnHeldItemReset;
    }
}