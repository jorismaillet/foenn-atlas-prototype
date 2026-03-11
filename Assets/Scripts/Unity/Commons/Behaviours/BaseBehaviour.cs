namespace Assets.Scripts.Unity.Commons.Behaviours
{
    using Assets.Scripts.Unity.Commons.Containers;
    using Assets.Scripts.Unity.Commons.Holders;
    using Assets.Scripts.Unity.Commons.Mutables;
    using Assets.Scripts.Unity.Commons.Utils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class BaseBehaviour : MonoBehaviour
    {
        private List<Action> removeListenerActions = new List<Action>();

        public void AddListener<T>(GameEvent<T> listenedEvent, UnityEvent<T> calledAction)
        {
            AddListener(listenedEvent, calledAction.Invoke);
        }

        public void AddListener(GameEvent listenedEvent, UnityEvent calledAction)
        {
            AddListener(listenedEvent, calledAction.Invoke);
        }

        public void AddListener(GameEvent listenedEvent, Action calledAction)
        {
            listenedEvent.AddListener(calledAction);
            removeListenerActions.Add(() =>
            {
                listenedEvent.RemoveListener(calledAction);
            });
        }

        public void SetMutable(IntContainer container, Mutable<int> mutableInt)
        {
            OnMutation(mutableInt, container.Initialize);
        }

        public void OnMutation<T>(Mutable<T> mutable, Action<T> Action)
        {
            AddListener(mutable.onChange, Action);
            Action.Invoke(mutable.Value);
        }

        protected void Set<Element>(BaseHolder holder, Element element)
        {
            //if (element != null) {
            try
            {
                ((IElementInitializer<Element>)holder).Initialize(element);
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"Error in gameobject {name}, component {GetType().Name}. Holder expects {holder.GetType()} but received {element.GetType()}", gameObject);
                throw e;
            }
        }

        protected void SetMutable<Element>(BaseHolder holder, Mutable<Element> element)
        {
            OnMutation(element, (e) => Set(holder, e));
        }

        public void OnMutation<T>(MutableList<T> mutable, Action<List<T>> Action)
        {
            AddListener(mutable.onChange, Action);
            Action.Invoke(mutable);
        }

        protected void Set<Element>(AbstractPrefabsContainer container, List<Element> list)
        {
            container.Initialize(list);
        }

        public void SetText(string text)
        {
            GetComponent<Text>().text = text;
        }

        protected float Normalize(float value, float maxValue)
        {
            return Math.Max(0.0F, Math.Min(maxValue, value)) / maxValue;
        }

        protected double Normalize(double value, double maxValue)
        {
            return Math.Max(0.0F, Math.Min(maxValue, value)) / maxValue;
        }

        protected void ScaleWidth(GameObject gameObject, GameObject content, float scalePercentage)
        {
            PanelUtil.SetWidth(gameObject.GetComponent<RectTransform>(), scalePercentage * PanelUtil.GetWidth(content.GetComponent<RectTransform>()));
        }

        public void ClearListeners()
        {
            removeListenerActions.ForEach(action =>
            {
                action.Invoke();
            });
            removeListenerActions.Clear();
        }

        protected void Behave(BehaviourAction action, string message = null)
        {
            switch (action)
            {
                case BehaviourAction.Show:
                    gameObject.SetActive(true);
                    if (message != null)
                    {
                        Text text = GetComponent<Text>();
                        if (text != null)
                        {
                            text.text = message;
                        }
                    }
                    break;
                case BehaviourAction.Hide:
                    gameObject.SetActive(false);
                    break;
                case BehaviourAction.Destroy:
                    Destroy(gameObject);
                    break;
                case BehaviourAction.DisableButton:
                    GetComponent<Button>().interactable = false;
                    break;
                case BehaviourAction.EnableButton:
                    GetComponent<Button>().interactable = true;
                    break;
                case BehaviourAction.ClearInputField:
                    GetComponent<InputField>().text = "";
                    break;
                case BehaviourAction.UnCheckToggle:
                    GetComponent<Toggle>().isOn = false;
                    break;
                case BehaviourAction.HideChildren:
                    foreach (Transform child in gameObject.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    break;
                case BehaviourAction.ToggleDisplay:
                    foreach (Transform child in gameObject.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    break;
                case BehaviourAction.InitializeHolder:
                    foreach (Transform child in gameObject.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    break;
            }
        }

        public void AddListener<T>(GameEvent<T> listenedEvent, Action<T> calledAction)
        {
            listenedEvent.AddListener(calledAction);
            removeListenerActions.Add(() =>
            {
                listenedEvent.RemoveListener(calledAction);
            });
        }

        public void AddListener<T1, T2>(GameEvent<T1, T2> listenedEvent, Action<T1, T2> calledAction)
        {
            listenedEvent.AddListener(calledAction);
            removeListenerActions.Add(() =>
            {
                listenedEvent.RemoveListener(calledAction);
            });
        }

        public void AddListener<T>(UnityEvent<T> trigger, Action<T> process)
        {
            void unityAction(T _)
            {
                process.Invoke(_);
            }
            trigger.AddListener(unityAction);
            removeListenerActions.Add(() =>
            {
                trigger.RemoveListener(unityAction);
            });
        }

        protected void SetGroupAlpha(float alpha)
        {
            GetComponent<CanvasGroup>().alpha = alpha;
        }

        protected void SetAlpha(Transform transform, float alpha)
        {
            ColorUtil.SetAlpha(transform.GetComponent<CanvasGroup>(), alpha);
        }

        public T GetElement<T>()
        {
            if (GetComponent<Holder<T>>() is Holder<T> holder)
            {
                return holder.element;
            }
            else
            {
                throw new Exception($"{name} is missing Holder of type {typeof(T)}");
            }
        }

        public void AddListener(UnityEvent trigger, Action process)
        {
            void unityAction()
            {
                process.Invoke();
            }
            trigger.AddListener(unityAction);
            removeListenerActions.Add(() =>
            {
                trigger.RemoveListener(unityAction);
            });
        }

        protected virtual void OnDestroy()
        {
            ClearListeners();
        }

        public GameObject AddGameObject(string prefabPath, string name = null)
        {
            return PrefabUtil.AddGameObject(prefabPath, name, transform);
        }

        public void DestroyChildrenGameObjects<T>() where T : MonoBehaviour
        {
            PrefabUtil.DestroyChildrenGameObjects<T>(transform);
        }

        public void DestroyAllChildren()
        {
            PrefabUtil.DestroyAllChildren(transform);
        }

        public void HideAllChildren()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public void SetLocalYPos(float y)
        {
            Vector3 localPos = transform.localPosition;
            localPos.y = y;
            transform.localPosition = localPos;
        }
    }
}
