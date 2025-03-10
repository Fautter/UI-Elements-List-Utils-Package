using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UIElementsListUtils
{
    public abstract class ElementsListUIManager<T> : MonoBehaviour
    {
        [SerializeField] protected GameObject elementsContainer;
        [SerializeField] protected GameObject listElementPrefab;
        protected List<UIListElement<T>> listElements;
        protected List<UIListElement<T>> filledElements;

        [Header("Filters")]
        [SerializeField][Min(1)] protected int searchMinChars = 2;
        protected string searchingName;

        protected List<UIListElement<T>> filteredElements;

        protected delegate void OnFilterDelegate();
        protected event OnFilterDelegate onFilter;

        void Awake()
        {
            listElements = GetElements<UIListElement<T>>(true);
            filledElements = new List<UIListElement<T>>();
            filteredElements = new List<UIListElement<T>>();
        }

        protected List<K> GetElements<K>(bool includeInactive)
        {
            return elementsContainer.GetComponentsInChildren<K>(includeInactive).ToList();
        }

        protected K CreateNewElement<K>()
        {
            return Instantiate(listElementPrefab, elementsContainer.transform).GetComponent<K>();
        }

        public void ShowFilledElements(bool show)
        {
            ShowElements(filledElements, show);
        }

        protected void ShowElements(List<UIListElement<T>> elements, bool show, bool clear = false)
        {
            foreach (UIListElement<T> element in elements)
            {
                if (clear)
                    element.Clear();

                element.gameObject.SetActive(show);
            }
        }

        public void FillElementsData(List<T> dataList, SendDataDelegate<T> dataAction, SendUIElementDelegate<T> elementAction, UnityAction actions)
        {
            filledElements.Clear();
            ShowElements(listElements, false, true);

            if (dataList.Count > 0)
            {
                for (int i = 0; i < dataList.Count; ++i)
                {
                    if (i >= listElements.Count)
                        listElements.Add(CreateNewElement<UIListElement<T>>());

                    listElements[i].Load(dataList[i], dataAction, elementAction, actions);
                    listElements[i].gameObject.SetActive(true);

                    filledElements.Add(listElements[i]);
                }
            }

            InvokeFilters();
        }

        protected void AddFilter(OnFilterDelegate filter)
        {
            if (onFilter != null)
            {
                if (!onFilter.GetInvocationList().Contains(filter))
                    onFilter += filter;
            }
            else
                onFilter += filter;
        }

        protected void RemoveFilter(OnFilterDelegate filter)
        {
            if (onFilter != null)
            {
                if (onFilter.GetInvocationList().Contains(filter))
                    onFilter -= filter;
            }
        }

        protected void InvokeFilters()
        {
            filteredElements.Clear();
            filteredElements.AddRange(filledElements);

            if (onFilter == null || onFilter.GetInvocationList() == null)
                ShowFilledElements(true);
            else
            {
                onFilter?.Invoke();
                ShowFilledElements(false);
                ShowElements(filteredElements, true);
            }
        }

        public void FilterByName(string search)
        {
            searchingName = search.Trim().ToLower();

            if (string.IsNullOrEmpty(searchingName))
                RemoveFilter(FilterElementsByName);
            else if (searchingName.Length > searchMinChars)
                AddFilter(FilterElementsByName);

            InvokeFilters();
        }

        protected abstract void FilterElementsByName();
    }
}
