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
            characters.SetActive(page == AlmanacPage.Characters);
            miscs.SetActive(page == AlmanacPage.Miscs);
        }
        public void SetContraptionEntries(ChoosingBlueprintViewData[] entries, bool commandBlockVisible)
        {
            standaloneContraptions.SetEntries(entries, commandBlockVisible);
            mobileContraptions.SetEntries(entries, commandBlockVisible);
        }
        public void SetEnemyEntries(AlmanacEntryViewData[] entries)
        {
            enemies.SetEntries(entries);
        }
        public void SetCharacterEntries(AlmanacEntryViewData[] entries)
        {
            characters.SetEntries(entries);
        }
        public void SetMiscEntries(AlmanacEntryViewData[] entries)
        {
            miscs.SetEntries(entries);
        }
        public void SetActiveContraptionEntry(Model prefab, string name, string description)
        {
            standaloneContraptions.SetActiveEntry(prefab, name, description);
            mobileContraptions.SetActiveEntry(prefab, name, description);
        }
        public void SetActiveEnemyEntry(Model prefab, string name, string description)
        {
            enemies.SetActiveEntry(prefab, name, description);
        }
        public void SetActiveCharacterEntry(Sprite sprite, string name, string description)
        {
            characters.SetActiveEntry(sprite, name, description);
        }
        public void SetActiveMiscEntry(Sprite sprite, string name, string description)
        {
            miscs.SetActiveEntry(sprite, name, description);
        }
        public void SetActiveMiscEntry(Model prefab, string name, string description)
        {
            miscs.SetActiveEntry(prefab, name, description);
        }
        private void Awake()
        {
            indexUI.OnButtonClick += type => OnIndexButtonClick?.Invoke(type);
            standaloneContraptions.OnEntryClick += index => OnContraptionEntryClick?.Invoke(index);
            mobileContraptions.OnEntryClick += index => OnContraptionEntryClick?.Invoke(index);
            enemies.OnEntryClick += index => OnEnemyEntryClick?.Invoke(index);
            characters.OnEntryClick += index => OnCharacterEntryClick?.Invoke(index);
            miscs.OnEntryClick += index => OnMiscEntryClick?.Invoke(index);

            indexUI.OnReturnClick += () => OnIndexReturnClick?.Invoke();
            standaloneContraptions.OnReturnClick += () => OnPageReturnClick?.Invoke();
            mobileContraptions.OnReturnClick += () => OnPageReturnClick?.Invoke();
            enemies.OnReturnClick += () => OnPageReturnClick?.Invoke();
            characters.OnReturnClick += () => OnPageReturnClick?.Invoke();
            miscs.OnReturnClick += () => OnPageReturnClick?.Invoke();
        }
        public event Action OnIndexReturnClick;
        public event Action OnPageReturnClick;
        public event Action<IndexAlmanacPage.ButtonType> OnIndexButtonClick;
        public event Action<int> OnContraptionEntryClick;
        public event Action<int> OnEnemyEntryClick;
        public event Action<int> OnCharacterEntryClick;
        public event Action<int> OnMiscEntryClick;
        [SerializeField]
        private IndexAlmanacPage indexUI;
        [SerializeField]
        private ContraptionAlmanacPage standaloneContraptions;
        [SerializeField]
        private ContraptionAlmanacPage mobileContraptions;
        [SerializeField]
        private MiscAlmanacPage enemies;
        [SerializeField]
        private MiscAlmanacPage characters;
        [SerializeField]
        private MiscAlmanacPage miscs;
        public enum AlmanacPage
        {
            Index,
            ContraptionsStandalone,
            ContraptionsMobile,
            Enemies,
            Characters,
            Miscs
        }
    }
}
