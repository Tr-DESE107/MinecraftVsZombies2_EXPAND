﻿using System;
using System.Collections.Generic;

namespace MVZ2Logic
{
    public enum PointerPhase
    {
        None,
        Press,
        Hold,
        Release
    }
    public struct PointerData : IEquatable<PointerData>
    {
        public int button;
        public int type;

        public override string ToString()
        {
            return $"button: {button}, type: {type}";
        }
        public override bool Equals(object obj)
        {
            return obj is PointerData data && Equals(data);
        }

        public bool Equals(PointerData other)
        {
            return button == other.button &&
                   type == other.type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(button, type);
        }

        public static bool operator ==(PointerData left, PointerData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointerData left, PointerData right)
        {
            return !(left == right);
        }
    }
    public struct PointerInteractionData : IEquatable<PointerInteractionData>
    {
        public PointerData pointer;
        public PointerInteraction interaction;
        public override string ToString()
        {
            return $"pointer: {pointer}\ninteraction: {interaction}";
        }
        public override bool Equals(object obj)
        {
            return obj is PointerInteractionData data && Equals(data);
        }

        public bool Equals(PointerInteractionData other)
        {
            return EqualityComparer<PointerData>.Default.Equals(pointer, other.pointer) &&
                   interaction == other.interaction;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(pointer, interaction);
        }

        public static bool operator ==(PointerInteractionData left, PointerInteractionData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointerInteractionData left, PointerInteractionData right)
        {
            return !(left == right);
        }
    }
    public enum PointerInteraction
    {
        Hover = 0,
        Enter = 1,
        Stay = 2,
        Exit = 3,

        Down = 4,
        Hold = 5,
        Up = 6,
        Click = 7,

        BeginDrag = 8,
        Drag = 9,
        EndDrag = 10,
        Drop = 11,

        Streak = 12,
        Release = 13,
        Key = 100
    }
    public static class PointerHelper
    {
        public static bool IsInvalidReleaseAction(this PointerInteractionData pointerParams)
        {
            if (pointerParams.pointer.type == PointerTypes.TOUCH)
            {
                if (pointerParams.interaction != PointerInteraction.Release)
                    return true;
            }
            else if (pointerParams.pointer.type == PointerTypes.MOUSE)
            {
                if (pointerParams.interaction != PointerInteraction.Down)
                    return true;
            }
            return false;
        }
        public static bool IsInvalidClickButton(this PointerInteractionData pointerParams)
        {
            if (pointerParams.pointer.type == PointerTypes.MOUSE)
            {
                if (pointerParams.pointer.button != MouseButtons.LEFT)
                    return true;
            }
            return false;
        }
    }
}
