﻿using System.Collections.Generic;
using MVZ2.GameContent.Effects;
using UnityEngine;

namespace MVZ2.Models
{
    public class NightmareDarknessModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var infos = Model.GetProperty<List<NightmareEyeInfo>>("Eyes");
            var timeout = Model.GetProperty<int>("Timeout");
            var time = 180 - timeout;
            float darknessRadius = Mathf.Lerp(maxRadius, 0, (30 - timeout) / 30f);
            darknessRenderer.SetFloat("_Radius", darknessRadius / maxRadius);

            if (infos != null)
            {
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    NightmareEye eye;
                    if (i >= eyes.Count)
                    {
                        eye = CreateEye(info);
                    }
                    else
                    {
                        eye = eyes[i];
                    }
                    var eyeTime = Mathf.Max(time - info.time, 0);
                    eye.SetTime(eyeTime);

                    float eyeRadius = (info.position - darknessCenter).magnitude;
                    float alpha = Mathf.Clamp01((darknessRadius - eyeRadius) / 100f * info.scale);
                    eye.SetAlpha(alpha);
                }
            }

        }
        private NightmareEye CreateEye(NightmareEyeInfo info)
        {
            NightmareEye eye = Instantiate(eyePrefab, eyesParent).GetComponent<NightmareEye>();
            eye.gameObject.SetActive(true);
            Transform trans = eye.transform;
            trans.localPosition = Lawn2TransPosition(info.position);

            Vector3 angles = trans.localEulerAngles;
            angles.z = info.angle;
            trans.localEulerAngles = angles;

            trans.localScale = Vector3.one * info.scale;

            Model.AddElement(eye.GetComponent<GraphicElement>());
            eyes.Add(eye);
            return eye;
        }
        [SerializeField]
        private Transform eyesParent;
        [SerializeField]
        private GameObject eyePrefab;
        [SerializeField]
        private RendererElement darknessRenderer;
        [SerializeField]
        private List<NightmareEye> eyes = new List<NightmareEye>();
        [SerializeField]
        private float maxRadius = 1200;
        [SerializeField]
        private Vector3 darknessCenter = new Vector3(510, 0, 300);
    }
}
