﻿using System;

namespace nadena.dev.ndmf.animator
{
    [ExcludeFromDocs]
    public abstract class VirtualNode
    {
        private Action _lastCacheObserver;

        internal VirtualNode()
        {
        }

        internal void Invalidate()
        {
            _lastCacheObserver?.Invoke();
            _lastCacheObserver = null;
        }

        internal T I<T>(T val)
        {
            Invalidate();
            return val;
        }

        internal void RegisterCacheObserver(Action observer)
        {
            if (observer != _lastCacheObserver && _lastCacheObserver != null)
            {
                _lastCacheObserver.Invoke();
            }

            _lastCacheObserver = observer;
        }
    }
}