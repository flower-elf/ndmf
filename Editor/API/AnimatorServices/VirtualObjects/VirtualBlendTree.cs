﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace nadena.dev.ndmf.animator
{
    public class VirtualBlendTree : VirtualMotion
    {
        private readonly BlendTree _tree;
        private VirtualChildMotion _motions;

        public sealed class VirtualChildMotion
        {
            public VirtualMotion Motion;
            public float CycleOffset;
            public string DirectBlendParameter;
            public bool Mirror;
            public float Threshold;
            public Vector2 Position;
            public float TimeScale;
        }

        private VirtualBlendTree(CloneContext context, BlendTree cloned)
        {
            _tree = cloned;

            context.DeferCall(() =>
            {
                Children = _tree.children.Select(m =>
                {
                    return new VirtualChildMotion
                    {
                        Motion = context.Clone(m.motion),
                        CycleOffset = m.cycleOffset,
                        DirectBlendParameter = m.directBlendParameter,
                        Mirror = m.mirror,
                        Threshold = m.threshold,
                        Position = m.position,
                        TimeScale = m.timeScale
                    };
                }).ToList();
            });
        }

        public static VirtualBlendTree Clone(
            CloneContext context,
            BlendTree tree
        )
        {
            if (tree == null) return null;
            if (context.TryGetValue(tree, out VirtualBlendTree existing)) return existing;

            var cloned = new BlendTree();
            EditorUtility.CopySerialized(tree, cloned);
            cloned.name = tree.name;

            return new VirtualBlendTree(context, cloned);
        }

        public string Name
        {
            get => _tree.name;
            set => _tree.name = value;
        }

        public string BlendParameter
        {
            get => _tree.blendParameter;
            set => _tree.blendParameter = value;
        }

        public string BlendParameterY
        {
            get => _tree.blendParameterY;
            set => _tree.blendParameterY = value;
        }

        public BlendTreeType BlendType
        {
            get => _tree.blendType;
            set => _tree.blendType = value;
        }

        public float MaxThreshold
        {
            get => _tree.maxThreshold;
            set => _tree.maxThreshold = value;
        }

        public float MinThreshold
        {
            get => _tree.minThreshold;
            set => _tree.minThreshold = value;
        }

        public bool UseAutomaticThresholds
        {
            get => _tree.useAutomaticThresholds;
            set => _tree.useAutomaticThresholds = value;
        }

        public List<VirtualChildMotion> Children { get; set; }

        protected override Motion Prepare(object context)
        {
            return _tree;
        }

        protected override void Commit(object context, Motion obj)
        {
            var commitContext = (CommitContext)context;
            var tree = (BlendTree)obj;

            tree.children = Children.Select(c =>
            {
                return new ChildMotion
                {
                    motion = commitContext.CommitObject(c.Motion),
                    cycleOffset = c.CycleOffset,
                    directBlendParameter = c.DirectBlendParameter,
                    mirror = c.Mirror,
                    threshold = c.Threshold,
                    position = c.Position,
                    timeScale = c.TimeScale
                };
            }).ToArray();
        }

        public override void Dispose()
        {
            Object.DestroyImmediate(_tree);
        }
    }
}