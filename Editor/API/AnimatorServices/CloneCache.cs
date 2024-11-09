﻿using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace nadena.dev.ndmf.animator
{
    public class CloneContext
    {
        public IPlatformAnimatorBindings PlatformBindings { get; private set; }
        private readonly Dictionary<object, object> _clones = new();

        public CloneContext(IPlatformAnimatorBindings platformBindings)
        {
            PlatformBindings = platformBindings;
        }

        public bool TryGetValue<T, U>(T key, out U value)
        {
            var rv = _clones.TryGetValue(key, out var tmp);

            if (rv) value = (U)tmp;
            else value = default;

            return rv;
        }

        public void Add<T, U>(T key, U value)
        {
            _clones.Add(key, value);
        }

        private U GetOrClone<T, U>(T key, Func<CloneContext, T, U> clone) where U : class
        {
            if (key == null) return null;
            if (TryGetValue(key, out U value)) return value;
            value = clone(this, key);
            Add(key, value);
            return value;
        }


        public VirtualState Clone(AnimatorState state)
        {
            return GetOrClone(state, VirtualState.Clone);
        }

        public VirtualMotion Clone(Motion m)
        {
            return GetOrClone(m, VirtualMotion.Clone);
        }

        public VirtualClip Clone(AnimationClip clip)
        {
            return GetOrClone(clip, VirtualClip.Clone);
        }
    }
}