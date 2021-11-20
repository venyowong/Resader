using System;
using System.Collections.Concurrent;

namespace Resader.Api.Helpers;
public static class LockHelper
{
    private static ConcurrentDictionary<string, object> _lockMap = new ConcurrentDictionary<string, object>();

    public static void Lock(string key, Action action)
    {
        if (!_lockMap.TryGetValue(key, out var l))
        {
            lock (_lockMap)
            {
                if (!_lockMap.TryGetValue(key, out l))
                {
                    l = new object();
                    _lockMap.TryAdd(key, l);
                }
            }
        }
        lock (l)
        {
            action.Invoke();
        }
    }
}
