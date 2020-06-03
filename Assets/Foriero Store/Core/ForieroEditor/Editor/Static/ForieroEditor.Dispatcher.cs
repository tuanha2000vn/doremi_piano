using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Threading;



namespace ForieroEditor
{
	[UnityEditor.InitializeOnLoad]
	public sealed class EditorDispatcher
	{

		public static Thread StartThread<T> (Action action)
		{
			var bigStackThread = new Thread (() => action (), 1024 * 1024);
			bigStackThread.Start ();
			return bigStackThread;
		}

		public static Thread StartThread<T> (Action<T> action, T parameterObject)
		{
			var bigStackThread = new Thread (() => action (parameterObject), 1024 * 1024);
			bigStackThread.Start ();
			return bigStackThread;
		}

		private struct Task
		{
			public Delegate Function;
			public object[] Arguments;

			public Task (Delegate function, object[] arguments)
			{
				Function = function;
				Arguments = arguments;
			}
		}

		private static Queue<Task> mTaskQueue = new Queue<Task> ();

		private static bool AreTasksAvailable {
			get { return mTaskQueue.Count > 0; }
		}

		static EditorDispatcher ()
		{
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.update += DispatchTasks;

			#endif
		}

		public static void Dispatch (Action function)
		{
			Dispatch (function, null);
		}

		public static void Dispatch<T> (Action<T> function, T parameter)
		{
			Dispatch (function, new object[1] { parameter });
		}

		public static void Dispatch (Delegate function, params object[] arguments)
		{
			#if UNITY_EDITOR
			lock (mTaskQueue) {
				mTaskQueue.Enqueue (new Task (function, arguments));
			}
			#else
			throw new System.NotSupportedException("Dispatch is not supported in the Unity Player!");
			#endif
		}

		public static void ClearTasks ()
		{
			#if UNITY_EDITOR
			if (AreTasksAvailable) {
				lock (mTaskQueue) {
					mTaskQueue.Clear ();
				}
			}
			#else
			throw new System.NotSupportedException("ClearTasks is not supported in the Unity Player!");
			#endif
		}

		private static void DispatchTasks ()
		{
			#if UNITY_EDITOR
			if (AreTasksAvailable) {
				lock (mTaskQueue) {
					foreach (Task task in mTaskQueue) {
						task.Function.DynamicInvoke (task.Arguments);
					}

					mTaskQueue.Clear ();
				}
			}
			#else
			throw new System.NotSupportedException("DispatchTasks is not supported in the Unity Player!");
			#endif
		}
	}
}
