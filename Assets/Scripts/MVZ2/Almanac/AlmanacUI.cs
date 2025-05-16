using System;
using MVZ2.Models;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class AlmanacUI : MonoBehaviour
    {
        public void DisplayPage(AlmanacPage page)
        {
            indexUI.SetActive(page == AlmanacPage.Index);
            standaloneContraptions.SetActive(page == AlmanacPage.ContraptionsStandalone);
            mobileContraptions.SetActive(page == AlmanacPage.ContraptionsMobile);
            enemies.SetActive(page == AlmanacPage.Enemies);
            artifacts.SetActive(page == AlmanacPage.Artifacts);
            miscs.SetActive(page == AlmanacPage.Miscs);
        }
        public void SetIndexArtifactVisible(bool visible)
        {
            indexUI.SetArtifactVisible(visible);
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
            mobileContraptions.OnEntryClick += index => OnContraptionEntryClick?.Invoke(index);
            mobileContraptions.OnCommandBlockClick += () => OnCommandBlockClick?.Invoke();
            enemies.OnEntryClick += index => OnEnemyEntryClick?.Invoke(index);
            enemies.OnZoomClick += () => OnEnemyZoomClick?.Invoke();
            artifacts.OnEntryClick += index => OnArtifactEntryClick?.Invoke(index);
            artifacts.OnZoomClick += () => OnArtifactZoomClick?.Invoke();
            miscs.OnGroupEntryClick += (groupIndex, entryIndex) => OnMiscGroupEntryClick?.Invoke(groupIndex, entryIndex);
            miscs.OnZoomClick += () => OnMiscZoomClick?.Invoke();

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
        public enum AlmanacPage
        {
            Index,
            ContraptionsStandalone,
            ContraptionsMobile,
            Enemies,
            Artifacts,
            Miscs
        }
    }
}
