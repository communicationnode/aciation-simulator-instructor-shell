using System.Numerics;
using System.Runtime.CompilerServices;

namespace InstructorShell.DataClasses {
    public static class MathAdvanced {

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public static T GetFromDiapazone<T>(T val, T oldMin, T oldMax, T newMin, T newMax) where T : INumber<T> {
            return (val - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public static T Lerp<T>(T target, T to, float step) where T : INumber<T> {

            float floatTarget = Convert.ToSingle(target);
            float floatTo = Convert.ToSingle(to);


            floatTarget += (floatTo - floatTarget) * step;

            return T.CreateChecked(floatTarget);
        }
    }
}
