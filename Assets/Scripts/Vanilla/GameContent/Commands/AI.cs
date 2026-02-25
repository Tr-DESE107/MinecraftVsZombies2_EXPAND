using System;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.IZombie;

// 添加对UnityEngine的引用
using UnityEngine;

namespace MVZ2.GameContent.Commands
{
    /// <summary>
    /// AI 命令定义
    /// </summary>
    [CommandDefinition(VanillaCommandNames.AI)]
    public class AI : CommandDefinition
    {
        public AI(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Invoke(string[] parameters)
        {
            if (parameters.Length < 1)
            {
                PrintLine("[AI] 用法: ai <start|stop>");
                return;
            }

            string command = parameters[0].ToLower();
            
            switch (command)
            {
                case "start":
                    StartAI();
                    break;
                case "stop":
                    StopAI();
                    break;
                default:
                    PrintLine($"[AI] 未知指令: ai {command}");
                    PrintLine("[AI] 可用指令: start, stop");
                    break;
            }
        }

        private void StartAI()
        {
            // 使用反射来获取TowerDefenseAgent实例，避免命名空间引用问题
            var agent = FindTowerDefenseAgent();
            if (agent != null)
            {
                // 使用反射调用HandleAICommand方法
                var method = agent.GetType().GetMethod("HandleAICommand");
                if (method != null)
                {
                    method.Invoke(agent, new object[] { "start", new string[0] });
                    PrintLine("[AI] AI 训练已启动");
                }
                else
                {
                    PrintLine("[AI] Error: TowerDefenseAgent 缺少 HandleAICommand 方法");
                }
            }
            else
            {
                PrintLine("[AI] Error: TowerDefenseAgent 实例不存在");
            }
        }

        private void StopAI()
        {
            // 使用反射来获取TowerDefenseAgent实例，避免命名空间引用问题
            var agent = FindTowerDefenseAgent();
            if (agent != null)
            {
                // 使用反射调用HandleAICommand方法
                var method = agent.GetType().GetMethod("HandleAICommand");
                if (method != null)
                {
                    method.Invoke(agent, new object[] { "stop", new string[0] });
                    PrintLine("[AI] AI 训练已停止");
                }
                else
                {
                    PrintLine("[AI] Error: TowerDefenseAgent 缺少 HandleAICommand 方法");
                }
            }
            else
            {
                PrintLine("[AI] Error: TowerDefenseAgent 实例不存在");
            }
        }

        /// <summary>
        /// 使用反射查找TowerDefenseAgent实例
        /// </summary>
        /// <returns>TowerDefenseAgent实例</returns>
        private object FindTowerDefenseAgent()
        {
            // 获取所有GameObject
            var allObjects = GameObject.FindObjectsOfType<MonoBehaviour>();
            
            // 遍历查找TowerDefenseAgent类型的实例
            foreach (var obj in allObjects)
            {
                if (obj.GetType().Name == "TowerDefenseAgent")
                {
                    return obj;
                }
            }
            
            return null;
        }
    }
}
