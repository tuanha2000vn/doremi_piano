﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ForieroEngine
{
    public class EventManager : MonoBehaviour
    {
        public struct Test : IEMEvent
        {
            public int i;
        }

        public interface IEMEvent
        {

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            UnityEngine.Object.DontDestroyOnLoad(new GameObject("EventManager").AddComponent<EventManager>());
        }

        //	void OnGUI ()
        //	{
        //		if (GUILayout.Button ("G")) {
        //			EventManager.TriggerEvent (new Test{ i = 5 });
        //		}
        //	}

        static bool limitQueueProcesing = false;
        static float queueProcessTime = 0.0f;
        static Queue m_eventQueue = new Queue();

        public delegate void EventDelegate<T>(T e) where T : IEMEvent;

        delegate void EventDelegate(IEMEvent e);

        static Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
        static Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
        static Dictionary<System.Delegate, System.Delegate> onceLookups = new Dictionary<System.Delegate, System.Delegate>();

        private static EventDelegate AddDelegate<T>(EventDelegate<T> del) where T : IEMEvent
        {
            // Early-out if we've already registered this delegate
            if (delegateLookup.ContainsKey(del))
                return null;

            // Create a new non-generic delegate which calls our generic one.
            // This is the delegate we actually invoke.
            EventDelegate internalDelegate = (e) => del((T)e);
            delegateLookup[del] = internalDelegate;

            EventDelegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel))
            {
                delegates[typeof(T)] = tempDel += internalDelegate;
            }
            else
            {
                delegates[typeof(T)] = internalDelegate;
            }

            return internalDelegate;
        }

        public static void AddListener<T>(EventDelegate<T> del) where T : IEMEvent
        {
            AddDelegate<T>(del);
        }

        public void AddListenerOnce<T>(EventDelegate<T> del) where T : IEMEvent
        {
            EventDelegate result = AddDelegate<T>(del);

            if (result != null)
            {
                // remember this is only called once
                onceLookups[result] = del;
            }
        }

        public static void RemoveListener<T>(EventDelegate<T> del) where T : IEMEvent
        {
            EventDelegate internalDelegate;
            if (delegateLookup.TryGetValue(del, out internalDelegate))
            {
                EventDelegate tempDel;
                if (delegates.TryGetValue(typeof(T), out tempDel))
                {
                    tempDel -= internalDelegate;
                    if (tempDel == null)
                    {
                        delegates.Remove(typeof(T));
                    }
                    else
                    {
                        delegates[typeof(T)] = tempDel;
                    }
                }

                delegateLookup.Remove(del);
            }
        }

        public static void RemoveAll()
        {
            delegates.Clear();
            delegateLookup.Clear();
            onceLookups.Clear();
        }

        public static bool HasListener<T>(EventDelegate<T> del) where T : IEMEvent
        {
            return delegateLookup.ContainsKey(del);
        }

        public static void TriggerEvent(IEMEvent e)
        {
            EventDelegate del;
            if (delegates.TryGetValue(e.GetType(), out del))
            {
                del.Invoke(e);

                // remove listeners which should only be called once
                foreach (EventDelegate k in delegates[e.GetType()].GetInvocationList())
                {
                    if (onceLookups.ContainsKey(k))
                    {
                        delegates[e.GetType()] -= k;

                        if (delegates[e.GetType()] == null)
                        {
                            delegates.Remove(e.GetType());
                        }

                        delegateLookup.Remove(onceLookups[k]);
                        onceLookups.Remove(k);
                    }
                }
            }
            else
            {
                //Debug.LogWarning ("Event: " + e.GetType () + " has no listeners");
            }
        }

        //Inserts the event into the current queue.
        public static bool QueueEvent(IEMEvent evt)
        {
            if (!delegates.ContainsKey(evt.GetType()))
            {
                //Debug.LogWarning ("EventManager: QueueEvent failed due to no listeners for event: " + evt.GetType ());
                return false;
            }

            m_eventQueue.Enqueue(evt);
            return true;
        }

        IEMEvent evt;

        //Every update cycle the queue is processed, if the queue processing is limited,
        //a maximum processing time per update can be set after which the events will have
        //to be processed next update loop.
        void Update()
        {
            float timer = 0.0f;
            while (m_eventQueue.Count > 0)
            {

                if (limitQueueProcesing)
                {

                    if (timer > queueProcessTime)
                    {
                        return;
                    }

                }

                evt = m_eventQueue.Dequeue() as IEMEvent;
                TriggerEvent(evt);

                if (limitQueueProcesing)
                {

                    timer += Time.deltaTime;

                }
            }
        }

        public void OnApplicationQuit()
        {
            RemoveAll();
            m_eventQueue.Clear();
        }
    }
}