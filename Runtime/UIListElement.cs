using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UIElementsListUtils
{
    public delegate void SendDataDelegate<T>(T data);

    public delegate void SendUIElementDelegate<T>(UIListElement<T> element);

    public abstract class UIListElement<T> : MonoBehaviour
    {
        protected T data;
        public T Data { get => data; }

        [SerializeField] protected TMP_Text dataText;

        protected event SendDataDelegate<T> sendDataDelegate;
        protected event SendUIElementDelegate<T> sendUIElementDelegate;
        protected UnityAction actions = null;

        public void SetSendData(SendDataDelegate<T> sendDataAction) => sendDataDelegate = sendDataAction;
        public void SetSendUIElement(SendUIElementDelegate<T> sendUIElementAction) => sendUIElementDelegate = sendUIElementAction;

        public virtual void Load(T data)
        {
            Load(data, null, null, null);
        }

        public virtual void Load(T data, SendDataDelegate<T> dataAction)
        {
            Load(data, dataAction, null, null);
        }

        public virtual void Load(T data, SendDataDelegate<T> dataAction, UnityAction actions)
        {
            Load(data, dataAction, null, actions);
        }

        public virtual void Load(T data, SendUIElementDelegate<T> elementAction)
        {
            Load(data, null, elementAction, null);
        }

        public virtual void Load(T data, SendUIElementDelegate<T> elementAction, UnityAction actions)
        {
            Load(data, null, elementAction, actions);
        }

        public virtual void Load(T data, SendDataDelegate<T> dataAction, SendUIElementDelegate<T> elementAction, UnityAction actions)
        {
            this.data = data;
            SetSendData(dataAction);
            SetSendUIElement(elementAction);
            this.actions = actions;
            FillDataText();
        }

        public virtual void Clear()
        {
            data = default;
            SetSendData(null);
            actions = null;
        }

        public virtual void SendData()
        {
            sendDataDelegate?.Invoke(data);
            actions?.Invoke();
        }

        public virtual void SendElement()
        {
            sendUIElementDelegate?.Invoke(this);
            actions?.Invoke();
        }

        protected abstract void FillDataText();
    }
}
