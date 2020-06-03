using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ForieroEditor.Extensions
{
	public static partial class ForieroEditorExtensions
	{
		public static int IndexOf<T> (
			this IEnumerable<T> list, 
			Predicate<T> condition)
		{               
			int i = -1;
			return list.Any (x => {
				i++;
				return condition (x);
			}) ? i : -1;
		}
	}
}
