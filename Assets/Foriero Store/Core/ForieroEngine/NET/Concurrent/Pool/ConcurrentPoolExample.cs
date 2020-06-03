using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.ObjectPool;

public class ConcurrentPoolExample
{

    public class MyObject : RecyclableObject
    {
        /// <summary>
        /// Some dummy property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Resets the object state.
        /// </summary>
        public override void Recycle()
        {
            this.Name = string.Empty;
        }
    }

    public static void Example()
    {
        var pool1 = new ConcurrentPool<MyObject>("Pool of MyObject");
        //var pool2 = new ConcurrentPool<MyObject>("Pool of MyObject", (x) => new MyObject());

        using (MyObject instance1 = pool1.Acquire())
        {
            // Do something with the instance
        }
        // The instance is released back to the pool 

        MyObject instance2 = pool1.Acquire();
        // Do something with the instance
        instance2.TryRelease();
    }

}
