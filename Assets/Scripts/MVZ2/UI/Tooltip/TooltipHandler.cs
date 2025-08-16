﻿using System;
using System.Collections.Generic;
using System.Text;
using MukioI18n;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipTarget, ITranslateComponent
    {
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        public event Action<TooltipHandler> OnPointerEnter;
        public event Action<TooltipHandler> OnPointerExit;

        public ITooltipAnchor Anchor => anchor;

        string ITranslateComponent.Key => text;
        string ITranslateComponent.Context => context;
        IEnumerable<string> ITranslateComponent.Keys => null;
        string ITranslateComponent.Comment => comment;
        string ITranslateComponent.Path
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Transform tr = transform;
                do
                {
                    sb.Insert(0, tr.name);
                    sb.Insert(0, "/");
                } while (tr = tr.parent);

                return sb.ToString();
            }
        }
        public TooltipAnchor anchor;
        public string context;
        public string comment;
        [TextArea]
        public string text;
    }
}
