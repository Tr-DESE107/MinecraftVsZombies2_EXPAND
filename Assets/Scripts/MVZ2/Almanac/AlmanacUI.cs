using System;
using System.Collections.Generic;
using MVZ2.Level.UI;
using MVZ2.Models;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class AlmanacUI : MonoBehaviour
    {
        public void DisplayPage(AlmanacPageType page)
        {
            indexUI.SetActive(page == AlmanacPageType.Index);
            standaloneContraptions.SetActive(page == AlmanacPageType.ContraptionsStandalone);
            mobileContraptions.SetActive(page == AlmanacPageType.ContraptionsMobile);
            enemies.SetActive(page == AlmanacPageType.Enemies);
            artifacts.SetActive(page == AlmanacPageType.Artifacts);
            miscs.SetActive(page == AlmanacPageType.Miscs);
        }
        public void SetIndexArtifactVisible(bool visible)
        {
            indexUI.SetArtifactVisible(visible);
        }
        public void ShowTooltip(ITooltipTarget target, TooltipViewData viewData)
        {
            var anchor = target.Anchor;
            if (!anchor || anchor.IsDisabled)
                return;
            tooltip.gameObject.SetActive(true);
            tooltip.SetData(anchor.transform, anchor.Pivot, viewData);
        }
        public void HideTooltip()
        {
            tooltip.gameObject.SetActive(false);
        }
        public AlmanacTagIcon GetTagIcon(AlmanacPageType page, int index)
        {
            switch (page)
            {
                case AlmanacPageType.ContraptionsStandalone:
                    return standaloneContraptions.GetTagIcon(index);
                case AlmanacPageType.ContraptionsMobile:
                    return mobileContraptions.GetTagIcon(index);
                case AlmanacPageType.Enemies:
                    return enemies.GetTagIcon(index);
                case AlmanacPageType.Artifacts:
                    return artifacts.GetTagIcon(index);
                case AlmanacPageType.Miscs:
                    return miscs.GetTagIcon(index);
            }
            return null;
        }
        public AlmanacTagIcon GetDescriptionIcon(AlmanacPageType page, string linkID)
        {
            switch (page)
            {
                case AlmanacPageType.ContraptionsStandalone:
                    return standaloneContraptions.GetDescriptionIcon(linkID);
                case AlmanacPageType.ContraptionsMobile:
                    return mobileContraptions.GetDescriptionIcon(linkID);
                case AlmanacPageType.Enemies:
                    return enemies.GetDescriptionIcon(linkID);
                case AlmanacPageType.Artifacts:
                    return artifacts.GetDescriptionIcon(linkID);
                case AlmanacPageType.Miscs:
                    return miscs.GetDescriptionIcon(linkID);
            }
            return null;
        }
        public void SetContraptionEntries(ChoosingBlueprintViewData[] entries, bool commandBlockVisible, ChoosingBlueprintViewData commandBlockViewData)
        {
            standaloneContraptions.SetEntries(entries, commandBlockVisible, commandBlockViewData);
            mobileContraptions.SetEntries(entries, commandBlockVisible, commandBlockViewData);
        }
        public void SetEnemyEntries(AlmanacEntryViewData[] entries)
        {
            enemies.SetEntries(entries);
        }
        public void SetArtifactEntries(AlmanacEntryViewData[] entries)
        {
            artifacts.SetEntries(entries);
        }
        public void SetMiscGroups(AlmanacEntryGroupViewData[] groups)
        {
            miscs.SetGroups(groups);
        }
        public void SetActiveContraptionEntry(Model prefab, Camera camera, string name, string description, string cost, string recharge)
        {
            standaloneContraptions.SetActiveEntry(prefab, camera, name, description, cost, recharge);
            mobileContraptions.SetActiveEntry(prefab, camera, name, description, cost, recharge);
        }
        public void SetActiveEnemyEntry(Model prefab, Camera camera, string name, string description)
        {
            enemies.SetActiveEntry(prefab, camera, name, description);
        }
        public void SetActiveArtifactEntry(Sprite sprite, string name, string description)
        {
            artifacts.SetActiveEntry(sprite, name, description, true, false);
        }
        public void SetActiveMiscEntry(Sprite sprite, string name, string description, bool sized = false, bool zoom = true)
        {
            miscs.SetActiveEntry(sprite, name, description, sized, zoom);
        }
        public void SetActiveMiscEntry(Model prefab, Camera camera, string name, string description)
        {
            miscs.SetActiveEntry(prefab, camera, name, description);
        }
        public void UpdateTagIcons(AlmanacPageType page, AlmanacTagIconViewData[] viewDatas)
        {
            switch (page)
            {
                case AlmanacPageType.ContraptionsStandalone:
                    standaloneContraptions.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacPageType.ContraptionsMobile:
                    mobileContraptions.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacPageType.Enemies:
                    enemies.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacPageType.Artifacts:
                    artifacts.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacPageType.Miscs:
                    miscs.UpdateTagIcons(viewDatas);
                    break;
            }
        }
        public void UpdateContraptionDescriptionIcons(AlmanacDescriptionTagViewData[] viewDatas)
        {
            standaloneContraptions.UpdateDescriptionIcons(viewDatas);
            mobileContraptions.UpdateDescriptionIcons(viewDatas);
        }
        public void UpdateEnemyDescriptionIcons(AlmanacDescriptionTagViewData[] viewDatas)
        {
            enemies.UpdateDescriptionIcons(viewDatas);
        }
        public void UpdateArtifactDescriptionIcons(AlmanacDescriptionTagViewData[] viewDatas)
        {
            artifacts.UpdateDescriptionIcons(viewDatas);
        }
        public void UpdateMiscDescriptionIcons(AlmanacDescriptionTagViewData[] viewDatas)
        {
            miscs.UpdateDescriptionIcons(viewDatas);
        }

        #region Ëõ·Å
        public void StartZoom(Sprite sprite)
        {
            zoomPage.Display(sprite);
        }
        public void StopZoom()
        {
            zoomPage.Hide();
        }
        public void SetZoomScale(float scale)
        {
            zoomPage.SetScale(scale);
        }
        public void SetZoomScaleSliderText(string text)
        {
            zoomPage.SetSliderText(text);
        }
        public void SetZoomScaleSliderValue(float value)
        {
            zoomPage.SetSliderValue(value);
        }
        #endregion
        private void Awake()
        {
            almanacPages.Add(AlmanacPageType.Index, indexUI);
            almanacPages.Add(AlmanacPageType.ContraptionsStandalone, standaloneContraptions);
            almanacPages.Add(AlmanacPageType.ContraptionsMobile, mobileContraptions);
            almanacPages.Add(AlmanacPageType.Enemies, enemies);
            almanacPages.Add(AlmanacPageType.Artifacts, artifacts);
            almanacPages.Add(AlmanacPageType.Miscs, miscs);


            foreach (var pair in almanacPages)
            {
                var type = pair.Key;
                var page = pair.Value;
                if (page is IndexAlmanacPage index)
                {
                    index.OnButtonClick += t => OnIndexButtonClick?.Invoke(t);
                    page.OnReturnClick += () => OnReturnClick?.Invoke(false);
                }
                else
                {
                    page.OnReturnClick += () => OnReturnClick?.Invoke(true);
                }
                if (page is BookAlmanacPage bookPage)
                {
                    bookPage.OnDescriptionIconEnter += id => OnDescriptionIconEnter?.Invoke(type, id);
                    bookPage.OnDescriptionIconExit += id => OnDescriptionIconExit?.Invoke(type, id);
                    bookPage.OnDescriptionIconDown += id => OnDescriptionIconDown?.Invoke(type, id);
                    bookPage.OnTagIconEnter += id => OnTagIconEnter?.Invoke(type, id);
                    bookPage.OnTagIconExit += id => OnTagIconExit?.Invoke(type, id);
                    bookPage.OnTagIconDown += id => OnTagIconDown?.Invoke(type, id);
                }

                if (page is ContraptionAlmanacPage contraptionPage)
                {
                    contraptionPage.OnEntryClick += index => OnEntryClick?.Invoke(type, index);
                    contraptionPage.OnCommandBlockClick += () => OnCommandBlockClick?.Invoke();
                }

                if (page is MiscAlmanacPage miscPage)
                {
                    miscPage.OnGroupEntryClick += (groupIndex, entryIndex) => OnGroupEntryClick?.Invoke(type, groupIndex, entryIndex);
                    miscPage.OnZoomClick += () => OnZoomClick?.Invoke(type);
                    miscPage.OnEntryClick += index => OnEntryClick?.Invoke(type, index);
                }
            }
            zoomPage.OnReturnClick += () => OnZoomReturnClick?.Invoke();
            zoomPage.OnScaleValueChanged += (v) => OnZoomScaleValueChanged?.Invoke(v);
        }
        public event Action<bool> OnReturnClick;
        public event Action<IndexAlmanacPage.ButtonType> OnIndexButtonClick;

        public event Action OnCommandBlockClick;
        public event Action<AlmanacPageType, int> OnEntryClick;
        public event Action<AlmanacPageType, int, int> OnGroupEntryClick;
        public event Action<AlmanacPageType> OnZoomClick;

        public event Action<AlmanacPageType, string> OnDescriptionIconEnter;
        public event Action<AlmanacPageType, string> OnDescriptionIconExit;
        public event Action<AlmanacPageType, string> OnDescriptionIconDown;
        public event Action<AlmanacPageType, int> OnTagIconEnter;
        public event Action<AlmanacPageType, int> OnTagIconExit;
        public event Action<AlmanacPageType, int> OnTagIconDown;

        public event Action OnZoomReturnClick;
        public event Action<float> OnZoomScaleValueChanged;

        private Dictionary<AlmanacPageType, AlmanacPage> almanacPages = new Dictionary<AlmanacPageType, AlmanacPage>();

        [SerializeField]
        private IndexAlmanacPage indexUI;
        [SerializeField]
        private ContraptionAlmanacPage standaloneContraptions;
        [SerializeField]
        private ContraptionAlmanacPage mobileContraptions;
        [SerializeField]
        private MiscAlmanacPage enemies;
        [SerializeField]
        private MiscAlmanacPage artifacts;
        [SerializeField]
        private MiscAlmanacPage miscs;
        [SerializeField]
        private AlmanacZoomPage zoomPage;
        [SerializeField]
        private Tooltip tooltip;
    }
    public enum AlmanacPageType
    {
        Index,
        ContraptionsStandalone,
        ContraptionsMobile,
        Enemies,
        Artifacts,
        Miscs
    }
}
