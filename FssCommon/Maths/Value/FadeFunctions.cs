
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Define several lists of double 100 elements long.
// Use an input value 0->1 *100 (int) to index the array and return a value
// Value lists can be constructed in excel

public enum FadeType { EaseInOut, EaseInOutPeak, ThreePhaseOne, ThreePhaseTwo, ThreePhaseThree, Linear25 };

public class FadeFunctions
{

    // Can receive an input value 0->1, so to cover the end values, 0 -> 100 (ie 101) output values.

    private static readonly FssFloat1DArray List_Linear25 = new FssFloat1DArray(new float[] {
        0.00f, 0.04f, 0.08f, 0.12f, 0.16f, 0.20f, 0.24f, 0.28f, 0.32f, 0.36f,
        0.40f, 0.44f, 0.48f, 0.52f, 0.56f, 0.60f, 0.64f, 0.68f, 0.72f, 0.76f,
        0.80f, 0.84f, 0.88f, 0.92f, 0.96f, 1.00f
    });

    private static readonly FssFloat1DArray List_EaseInOut = new FssFloat1DArray(new float[] {
        0.00000f, 0.00025f, 0.00099f, 0.00222f, 0.00394f, 0.00616f, 0.00886f, 0.01204f, 0.01571f, 0.01985f,
        0.02447f, 0.02956f, 0.03511f, 0.04112f, 0.04759f, 0.05450f, 0.06185f, 0.06963f, 0.07784f, 0.08646f,
        0.09549f, 0.10492f, 0.11474f, 0.12494f, 0.13552f, 0.14645f, 0.15773f, 0.16934f, 0.18129f, 0.19355f,
        0.20611f, 0.21896f, 0.23209f, 0.24548f, 0.25912f, 0.27300f, 0.28711f, 0.30143f, 0.31594f, 0.33063f,
        0.34549f, 0.36050f, 0.37566f, 0.39093f, 0.40631f, 0.42178f, 0.43733f, 0.45295f, 0.46860f, 0.48429f,
        0.50000f, 0.51571f, 0.53140f, 0.54705f, 0.56267f, 0.57822f, 0.59369f, 0.60907f, 0.62434f, 0.63950f,
        0.65451f, 0.66937f, 0.68406f, 0.69857f, 0.71289f, 0.72700f, 0.74088f, 0.75452f, 0.76791f, 0.78104f,
        0.79389f, 0.80645f, 0.81871f, 0.83066f, 0.84227f, 0.85355f, 0.86448f, 0.87506f, 0.88526f, 0.89508f,
        0.90451f, 0.91354f, 0.92216f, 0.93037f, 0.93815f, 0.94550f, 0.95241f, 0.95888f, 0.96489f, 0.97044f,
        0.97553f, 0.98015f, 0.98429f, 0.98796f, 0.99114f, 0.99384f, 0.99606f, 0.99778f, 0.99901f, 1.00000f
    });

    private static readonly FssFloat1DArray List_EaseInOutPeak = new FssFloat1DArray(new float[] {
        0.00000f, 0.00099f, 0.00394f, 0.00886f, 0.01571f, 0.02447f, 0.03511f, 0.04759f, 0.06185f, 0.07784f,
        0.09549f, 0.11474f, 0.13552f, 0.15773f, 0.18129f, 0.20611f, 0.23209f, 0.25912f, 0.28711f, 0.31594f,
        0.34549f, 0.37566f, 0.40631f, 0.43733f, 0.46860f, 0.50000f, 0.53140f, 0.56267f, 0.59369f, 0.62434f,
        0.65451f, 0.68406f, 0.71289f, 0.74088f, 0.76791f, 0.79389f, 0.81871f, 0.84227f, 0.86448f, 0.88526f,
        0.90451f, 0.92216f, 0.93815f, 0.95241f, 0.96489f, 0.97553f, 0.98429f, 0.99114f, 0.99606f, 0.99901f,
        1.00000f, 0.99901f, 0.99606f, 0.99114f, 0.98429f, 0.97553f, 0.96489f, 0.95241f, 0.93815f, 0.92216f,
        0.90451f, 0.88526f, 0.86448f, 0.84227f, 0.81871f, 0.79389f, 0.76791f, 0.74088f, 0.71289f, 0.68406f,
        0.65451f, 0.62434f, 0.59369f, 0.56267f, 0.53140f, 0.50000f, 0.46860f, 0.43733f, 0.40631f, 0.37566f,
        0.34549f, 0.31594f, 0.28711f, 0.25912f, 0.23209f, 0.20611f, 0.18129f, 0.15773f, 0.13552f, 0.11474f,
        0.09549f, 0.07784f, 0.06185f, 0.04759f, 0.03511f, 0.02447f, 0.01571f, 0.00886f, 0.00294f, 0.00000f
    });

    private static readonly FssFloat1DArray List_3Phase1 = new FssFloat1DArray(new float[] {
        1.00000f, 0.99901f, 0.99606f, 0.99114f, 0.98429f, 0.97553f, 0.96489f, 0.95241f, 0.93815f, 0.92216f,
        0.90451f, 0.88526f, 0.86448f, 0.84227f, 0.81871f, 0.79389f, 0.76791f, 0.74088f, 0.71289f, 0.68406f,
        0.65451f, 0.62434f, 0.59369f, 0.56267f, 0.53140f, 0.50000f, 0.46860f, 0.43733f, 0.40631f, 0.37566f,
        0.34549f, 0.31594f, 0.28711f, 0.25912f, 0.23209f, 0.20611f, 0.18129f, 0.15773f, 0.13552f, 0.11474f,
        0.09549f, 0.07784f, 0.06185f, 0.04759f, 0.03511f, 0.02447f, 0.01571f, 0.00886f, 0.00394f, 0.00099f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
    });

    private static readonly FssFloat1DArray List_3Phase2 = new FssFloat1DArray(new float[] {
        0.00000f, 0.00099f, 0.00394f, 0.00886f, 0.01571f, 0.02447f, 0.03511f, 0.04759f, 0.06185f, 0.07784f,
        0.09549f, 0.11474f, 0.13552f, 0.15773f, 0.18129f, 0.20611f, 0.23209f, 0.25912f, 0.28711f, 0.31594f,
        0.34549f, 0.37566f, 0.40631f, 0.43733f, 0.46860f, 0.50000f, 0.53140f, 0.56267f, 0.59369f, 0.62434f,
        0.65451f, 0.68406f, 0.71289f, 0.74088f, 0.76791f, 0.79389f, 0.81871f, 0.84227f, 0.86448f, 0.88526f,
        0.90451f, 0.92216f, 0.93815f, 0.95241f, 0.96489f, 0.97553f, 0.98429f, 0.99114f, 0.99606f, 0.99901f,
        1.00000f, 1.00000f, 0.99606f, 0.99114f, 0.98429f, 0.97553f, 0.96489f, 0.95241f, 0.93815f, 0.92216f,
        0.90451f, 0.88526f, 0.86448f, 0.84227f, 0.81871f, 0.79389f, 0.76791f, 0.74088f, 0.71289f, 0.68406f,
        0.65451f, 0.62434f, 0.59369f, 0.56267f, 0.53140f, 0.50000f, 0.46860f, 0.43733f, 0.40631f, 0.37566f,
        0.34549f, 0.31594f, 0.28711f, 0.25912f, 0.23209f, 0.20611f, 0.18129f, 0.15773f, 0.13552f, 0.11474f,
        0.09549f, 0.07784f, 0.06185f, 0.04759f, 0.03511f, 0.02447f, 0.01571f, 0.00886f, 0.00394f, 0.00000f,
    });

    private static readonly FssFloat1DArray List_3Phase3 = new FssFloat1DArray(new float[] {
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f, 0.00000f,
        0.00000f, 0.00099f, 0.00394f, 0.00886f, 0.01571f, 0.02447f, 0.03511f, 0.04759f, 0.06185f, 0.07784f,
        0.09549f, 0.11474f, 0.13552f, 0.15773f, 0.18129f, 0.20611f, 0.23209f, 0.25912f, 0.28711f, 0.31594f,
        0.34549f, 0.37566f, 0.40631f, 0.43733f, 0.46860f, 0.50000f, 0.53140f, 0.56267f, 0.59369f, 0.62434f,
        0.65451f, 0.68406f, 0.71289f, 0.74088f, 0.76791f, 0.79389f, 0.81871f, 0.84227f, 0.86448f, 0.88526f,
        0.90451f, 0.92216f, 0.93815f, 0.95241f, 0.96489f, 0.97553f, 0.98429f, 0.99114f, 0.99606f, 1.00000f,
    });

    private static FssFloat1DArray GetList(FadeType listType)
    {
        switch (listType)
        {
            case FadeType.EaseInOut:       return List_EaseInOut;
            case FadeType.EaseInOutPeak:   return List_EaseInOutPeak;
            case FadeType.ThreePhaseOne:   return List_3Phase1;
            case FadeType.ThreePhaseTwo:   return List_3Phase2;
            case FadeType.ThreePhaseThree: return List_3Phase3;
            case FadeType.Linear25:        return List_Linear25;
            default:
                throw new ArgumentOutOfRangeException(nameof(listType), "Invalid FadeType");
        }
    }

    public static float ClosestValueForFraction(FadeType listType, float fraction)
    {
        var list = GetList(listType);
        int index = list.IndexForFraction(fraction);
        return list[index];
    }

    public static float InterpolatedValueForFraction(FadeType listType, float fraction)
    {
        var list = GetList(listType);
        return list.InterpolateAtFraction(fraction);
    }
}
