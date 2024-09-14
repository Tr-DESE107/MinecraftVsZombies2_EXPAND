using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Serialization;

namespace PVZEngine.LevelManagement
{
    public class BuffList
    {
        public bool AddBuff(Buff buff)
        {
            if (buff == null)
                return false;
            buffs.Add(buff);
            return true;
        }
        public bool RemoveBuff(Buff buff)
        {
            if (buff == null)
                return false;
            if (buffs.Remove(buff))
            {
                buff.RemoveFromTarget();
                return true;
            }
            return false;
        }
        public int RemoveBuffs(IEnumerable<Buff> buffs)
        {
            if (buffs == null)
                return 0;
            int count = 0;
            foreach (var buff in buffs)
            {
                count += RemoveBuff(buff) ? 1 : 0;
            }
            return count;
        }
        public bool HasBuff<T>() where T : BuffDefinition
        {
            return buffs.Any(b => b.Definition is T);
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
        }
        public Buff[] GetBuffs<T>() where T : BuffDefinition
        {
            return buffs.Where(b => b.Definition is T).ToArray();
        }
        public Buff[] GetAllBuffs()
        {
            return buffs.ToArray();
        }
        public object CalculateProperty(string name, object value)
        {
            foreach (var buff in buffs)
            {
                foreach (var modi in buff.GetModifiers())
                {
                    if (modi.PropertyName != name)
                        continue;
                    value = modi.CalculateProperty(buff, value);
                }
            }
            return value;
        }
        public SerializableBuffList ToSerializable()
        {
            return new SerializableBuffList()
            {
                buffs = buffs.ConvertAll(b => b.Serialize())
            };
        }
        public static BuffList FromSerializable(SerializableBuffList buffList, Level level)
        {
            return new BuffList()
            {
                buffs = buffList.buffs.ConvertAll(b => Buff.Deserialize(b, level))
            };
        }
        private List<Buff> buffs = new List<Buff>();
    }
}
