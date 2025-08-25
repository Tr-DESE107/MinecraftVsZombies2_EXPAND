using MVZ2.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Models;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void Awake_Model()
        {
            areaModelInterface = new AreaModelInterface(this);
        }
        private void InitLevelEngine_Model(LevelEngine level, NamespaceID areaID, NamespaceID stageID)
        {
            CreateLevelModel(areaID, stageID);
        }
        private void WriteToSerializable_Model(SerializableLevelController seri)
        {
            seri.model = model.ToSerializable();
        }
        private void ReadFromSerializable_Model(SerializableLevelController seri)
        {
            model.LoadFromSerializable(seri.model);
        }

        #region 初始化模型
        private void CreateLevelModel(NamespaceID areaId, NamespaceID stageID)
        {
            var areaDef = Game.GetAreaDefinition(areaId);
            if (areaDef == null)
                return;
            var modelID = areaDef.GetModelID();
            if (modelID == null)
                return;
            var modelPrefab = Resources.GetAreaModel(modelID);
            if (modelPrefab == null)
                return;
            model = Instantiate(modelPrefab.gameObject, modelRoot).GetComponent<AreaModel>();
            if (model)
            {
                model.Init(modelID, GetCamera());
                var stageDef = Game.GetStageDefinition(stageID);
                if (stageDef != null)
                {
                    SetModelPreset(stageDef.GetModelPreset());
                }
            }
        }
        #endregion

        #region 修改模型
        public void SetModelPreset(string name)
        {
            if (!model)
                return;
            model.SetPreset(name);
        }
        #endregion

        #region 获取模型
        public AreaModel GetAreaModel()
        {
            return model;
        }
        public IModelInterface GetAreaModelInterface()
        {
            return areaModelInterface;
        }
        #endregion

        #region 动画
        public void TriggerModelAnimator(string name)
        {
            if (!model)
                return;
            model.TriggerAnimator(name);
        }
        public void SetModelAnimatorBool(string name, bool value)
        {
            if (!model)
                return;
            model.SetAnimatorBool(name, value);
        }
        public void SetModelAnimatorInt(string name, int value)
        {
            if (!model)
                return;
            model.SetAnimatorInt(name, value);
        }
        public void SetModelAnimatorFloat(string name, float value)
        {
            if (!model)
                return;
            model.SetAnimatorFloat(name, value);
        }
        #endregion

        #region 属性字段
        private AreaModel model;
        private IModelInterface areaModelInterface;

        [Header("Model")]
        [SerializeField]
        private Transform modelRoot;
        #endregion
    }
}
