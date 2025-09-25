#nullable enable

using PVZEngine.Models;

namespace PVZEngine.Buffs
{
    public interface IModeledBuffTarget : IHasModel, IBuffTarget
    {
        IModelInterface? IBuffTarget.GetInsertedModel(NamespaceID key) => this.GetChildModel(key);
    }
}