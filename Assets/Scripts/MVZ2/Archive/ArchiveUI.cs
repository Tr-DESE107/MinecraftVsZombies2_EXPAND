using System;
using UnityEngine;

namespace MVZ2.Archives
{
    public class ArchiveUI : MonoBehaviour
    {
        public void DisplayPage(Page page)
        {
            indexUI.SetActive(page == Page.Index);
            details.SetActive(page == Page.Details);
            simulation.SetActive(page == Page.Simulation);
        }
        public void SetIndexSearch(string content)
        {
            indexUI.SetSearch(content);
        }
        public void SetIndexTags(ArchiveTagViewData[] tags)
        {
            indexUI.UpdateTags(tags);
        }
        public void SetIndexTalks(string[] talks)
        {
            indexUI.UpdateTalks(talks);
        }
        public void UpdateDetails(ArchiveDetailsViewData viewData)
        {
            details.UpdateDetails(viewData);
        }
        public void SetSimulationBackground(Sprite background)
        {
            simulation.SetBackground(background);
        }
        private void Awake()
        {
            indexUI.OnReturnClick += () => OnIndexReturnClick?.Invoke();
            indexUI.OnSearchEndEdit += value => OnSearchEndEdit?.Invoke(value);
            indexUI.OnTagValueChanged += (index, value) => OnTalkTagValueChanged?.Invoke(index, value);
            indexUI.OnTalkClick += value => OnTalkEntryClick?.Invoke(value);

            details.OnReturnClick += () => OnDetailsReturnClick?.Invoke();
            details.OnPlayClick += () => OnDetailsPlayClick?.Invoke();
        }
        public event Action OnIndexReturnClick;
        public event Action<string> OnSearchEndEdit;
        public event Action<int, bool> OnTalkTagValueChanged;
        public event Action<int> OnTalkEntryClick;

        public event Action OnDetailsReturnClick;
        public event Action OnDetailsPlayClick;

        [SerializeField]
        private IndexArchivePage indexUI;
        [SerializeField]
        private DetailsArchivePage details;
        [SerializeField]
        private SimulationArchivePage simulation;
        public enum Page
        {
            Index,
            Details,
            Simulation
        }
    }
}
