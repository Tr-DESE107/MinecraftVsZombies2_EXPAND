using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2Logic.Artifacts;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2Logic.Entities
{
    public class MetaArtifactDefinition : ArtifactDefinition
    {
        public MetaArtifactDefinition(IArtifactMeta meta, string nsp, string name) : base(nsp, name)
        {
            spriteReference = meta.Sprite;
            buffID = meta.BuffID;
        }

        public override SpriteReference GetSpriteReference()
        {
            return spriteReference;
        }

        public override NamespaceID GetBuffID()
        {
            return buffID;
        }
        private SpriteReference spriteReference;
        private NamespaceID buffID;
    }
}
