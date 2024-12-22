using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ArtifactChoosingDialog : MonoBehaviour
    {
        public void UpdateArtifacts(ArtifactSelectItemViewData[] items)
        {
            artifactList.updateList(items.Length, (i, obj) =>
            {
                var item = obj.GetComponent<ArtifactSelectItem>();
                item.UpdateItem(items[i]);
            },
            obj =>
            {
                var item = obj.GetComponent<ArtifactSelectItem>();
                item.OnClick += OnItemClickedCallback;
            },
            obj =>
            {
                var item = obj.GetComponent<ArtifactSelectItem>();
                item.OnClick -= OnItemClickedCallback;
            });
        }
        public ArtifactSelectItem GetArtifactSelectItem(int index)
        {
            return artifactList.getElement<ArtifactSelectItem>(index);
        }
        private void Awake()
        {
            backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
        }
        private void OnItemClickedCallback(ArtifactSelectItem item)
        {
            OnItemClicked?.Invoke(artifactList.indexOf(item));
        }
        public event Action<int> OnItemClicked;
        public event Action OnBackButtonClicked;
        [SerializeField]
        private ElementList artifactList;
        [SerializeField]
        private Button backButton;
    }
}
