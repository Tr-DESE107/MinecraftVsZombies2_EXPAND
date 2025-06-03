using System;
using MVZ2.Level.UI;
using MVZ2.Models;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class AlmanacUI : MonoBehaviour
    {
        public void DisplayPage(AlmanacUIPage page)
        {
            indexUI.SetActive(page == AlmanacUIPage.Index);
            standaloneContraptions.SetActive(page == AlmanacUIPage.ContraptionsStandalone);
            mobileContraptions.SetActive(page == AlmanacUIPage.ContraptionsMobile);
            enemies.SetActive(page == AlmanacUIPage.Enemies);
            artifacts.SetActive(page == AlmanacUIPage.Artifacts);
            miscs.SetActive(page == AlmanacUIPage.Miscs);
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
        public AlmanacTagIcon GetTagIcon(AlmanacUIPage page, int index)
        {
            switch (page)
            {
                case AlmanacUIPage.ContraptionsStandalone:
                    return standaloneContraptions.GetTagIcon(index);
                case AlmanacUIPage.ContraptionsMobile:
                    return mobileContraptions.GetTagIcon(index);
                case AlmanacUIPage.Enemies:
                    return enemies.GetTagIcon(index);
                case AlmanacUIPage.Artifacts:
                    return artifacts.GetTagIcon(index);
                case AlmanacUIPage.Miscs:
                    return miscs.GetTagIcon(index);
            }
            return null;
        }
        public AlmanacTagIcon GetDescriptionIcon(AlmanacUIPage page, string linkID)
        {
            switch (page)
            {
                case AlmanacUIPage.ContraptionsStandalone:
                    return standaloneContraptions.GetDescriptionIcon(linkID);
                case AlmanacUIPage.ContraptionsMobile:
                    return mobileContraptions.GetDescriptionIcon(linkID);
                case AlmanacUIPage.Enemies:
                    return enemies.GetDescriptionIcon(linkID);
                case AlmanacUIPage.Artifacts:
                    return artifacts.GetDescriptionIcon(linkID);
                case AlmanacUIPage.Miscs:
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
        public void UpdateTagIcons(AlmanacUIPage page, AlmanacTagIconViewData[] viewDatas)
        {
            switch (page)
            {
                case AlmanacUIPage.ContraptionsStandalone:
                    standaloneContraptions.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacUIPage.ContraptionsMobile:
                    mobileContraptions.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacUIPage.Enemies:
                    enemies.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacUIPage.Artifacts:
                    artifacts.UpdateTagIcons(viewDatas);
                    break;
                case AlmanacUIPage.Miscs:
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
            indexUI.OnButtonClick += type => OnIndexButtonClick?.Invoke(type);

            standaloneContraptions.OnEntryClick += index => OnContraptionEntryClick?.Invoke(index);
            standaloneContraptions.OnCommandBlockClick += () => OnCommandBlockClick?.Invoke();
            standaloneContraptions.OnDescriptionIconEnter += id => OnDescriptionIconEnter?.Invoke(AlmanacUIPage.ContraptionsStandalone, id);
            standaloneContraptions.OnDescriptionIconExit += id => OnDescriptionIconExit?.Invoke(AlmanacUIPage.ContraptionsStandalone, id);
            standaloneContraptions.OnTagIconEnter += id => OnTagIconEnter?.Invoke(AlmanacUIPage.ContraptionsStandalone, id);
            standaloneContraptions.OnTagIconExit += id => OnTagIconExit?.Invoke(AlmanacUIPage.ContraptionsStandalone, id);

            mobileContraptions.OnEntryClick += index => OnContraptionEntryClick?.Invoke(index);
            mobileContraptions.OnCommandBlockClick += () => OnCommandBlockClick?.Invoke();
            mobileContraptions.OnDescriptionIconEnter += id => OnDescriptionIconEnter?.Invoke(AlmanacUIPage.ContraptionsMobile, id);
            mobileContraptions.OnDescriptionIconExit += id => OnDescriptionIconExit?.Invoke(AlmanacUIPage.ContraptionsMobile, id);
            mobileContraptions.OnTagIconEnter += id => OnTagIconEnter?.Invoke(AlmanacUIPage.ContraptionsMobile, id);
            mobileContraptions.OnTagIconExit += id => OnTagIconExit?.Invoke(AlmanacUIPage.ContraptionsMobile, id);

            enemies.OnEntryClick += index => OnEnemyEntryClick?.Invoke(index);
            enemies.OnZoomClick += () => OnEnemyZoomClick?.Invoke();
            enemies.OnDescriptionIconEnter += id => OnDescriptionIconEnter?.Invoke(AlmanacUIPage.Enemies, id);
            enemies.OnDescriptionIconExit += id => OnDescriptionIconExit?.Invoke(AlmanacUIPage.Enemies, id);
            enemies.OnTagIconEnter += id => OnTagIconEnter?.Invoke(AlmanacUIPage.Enemies, id);
            enemies.OnTagIconExit += id => OnTagIconExit?.Invoke(AlmanacUIPage.Enemies, id);

            artifacts.OnEntryClick += index => OnArtifactEntryClick?.Invoke(index);
            artifacts.OnZoomClick += () => OnArtifactZoomClick?.Invoke();
            artifacts.OnDescriptionIconEnter += id => OnDescriptionIconEnter?.Invoke(AlmanacUIPage.Artifacts, id);
            artifacts.OnDescriptionIconExit += id => OnDescriptionIconExit?.Invoke(AlmanacUIPage.Artifacts, id);
            artifacts.OnTagIconEnter += id => OnTagIconEnter?.Invoke(AlmanacUIPage.Artifacts, id);
            artifacts.OnTagIconExit += id => OnTagIconExit?.Invoke(AlmanacUIPage.Artifacts, id);

            miscs.OnGroupEntryClick += (groupIndex, entryIndex) => OnMiscGroupEntryClick?.Invoke(groupIndex, entryIndex);
            miscs.OnZoomClick += () => OnMiscZoomClick?.Invoke();
            miscs.OnDescriptionIconEnter += id => OnDescriptionIconEnter?.Invoke(AlmanacUIPage.Miscs, id);
            miscs.OnDescriptionIconExit += id => OnDescriptionIconExit?.Invoke(AlmanacUIPage.Miscs, id);
            miscs.OnTagIconEnter += id => OnTagIconEnter?.Invoke(AlmanacUIPage.Miscs, id);
            miscs.OnTagIconExit += id => OnTagIconExit?.Invoke(AlmanacUIPage.Miscs, id);

            indexUI.OnReturnClick += () => OnIndexReturnClick?.Invoke();
            standaloneContraptions.OnReturnClick += () => OnPageReturnClick?.Invoke();
            mobileContraptions.OnReturnClick += () => OnPageReturnClick?.Invoke();
            enemies.OnReturnClick += () => OnPageReturnClick?.Invoke();
            artifacts.OnReturnClick += () => OnPageReturnClick?.Invoke();
            miscs.OnReturnClick += () => OnPageReturnClick?.Invoke();

            zoomPage.OnReturnClick += () => OnZoomReturnClick?.Invoke();
            zoomPage.OnScaleValueChanged += (v) => OnZoomScaleValueChanged?.Invoke(v);
        }
        public event Action OnIndexReturnClick;
        public event Action OnPageReturnClick;
        public event Action<IndexAlmanacPage.ButtonType> OnIndexButtonClick;
        public event Action<int> OnContraptionEntryClick;
        public event Action OnCommandBlockClick;
        public event Action<int> OnEnemyEntryClick;
        public event Action OnEnemyZoomClick;
        public event Action<int> OnArtifactEntryClick;
        public event Action OnArtifactZoomClick;
        public event Action<int, int> OnMiscGroupEntryClick;
        public event Action OnMiscZoomClick;

        public event Action<AlmanacUIPage, string> OnDescriptionIconEnter;
        public event Action<AlmanacUIPage, string> OnDescriptionIconExit;
        public event Action<AlmanacUIPage, int> OnTagIconEnter;
        public event Action<AlmanacUIPage, int> OnTagIconExit;

        public event Action OnZoomReturnClick;
        public event Action<float> OnZoomScaleValueChanged;
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
    public enum AlmanacUIPage
    {
        Index,
        ContraptionsStandalone,
        ContraptionsMobile,
        Enemies,
        Artifacts,
        Miscs
    }
}
