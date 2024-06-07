// https://github.com/stryker-mutator/stryker-net/issues/305

namespace TargetProject.Defects
{
    public class UnsafeCode
    {
        public int GetElement(int[] arrayOfInt, int index)
        {
            unsafe
            {
                // Must pin object on heap so that it doesn't move while using interior pointers.
                fixed (int* p = &arrayOfInt[0])
                {
                    return *(p + index);
                }
            }
        }
    }
}
