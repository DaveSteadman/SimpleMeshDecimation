


using SkiaSharp;
using System;
using System.Collections.Generic;

public static partial class KoreMeshOperations
{
    // Usage: KoreMeshOperations.DecimateRandom(mesh, 0.5f);
    public static void DecimateRandom(KoreMesh mesh, float percentActive)
    {
        // Get a list of all the point in the mesh, filter out the anchor points
        // Then determine the number of points to keep and randomly split the list
        // into two lists, one to keep and one to remove
        // Mark each set of points accordingly.

        // 1 - Get a list of all the points in the meshdata
        List<KoreMeshPoint> allPoints = mesh.GetPointsAsList();

        // 2 - Filter out the anchor points
        List<KoreMeshPoint> pointsToAssess = new List<KoreMeshPoint>();
        foreach (KoreMeshPoint point in allPoints)
            if (!point.anchorPoint) pointsToAssess.Add(point);


        // 3 - Determine the number of points to keep
        int numPoints = pointsToAssess.Count;
        int numToKeep = (int)(numPoints * percentActive);

        // 4 - Randomly select points to keep - noting that the new state information
        //     has to be organised enough to write back into the meshdata.
        Random random = new Random();
        while (pointsToAssess.Count > numToKeep)
        {
            // Find a random point in the list
            int index = random.Next(0, pointsToAssess.Count);

            // Mark the point as not in use
            mesh.SetActiveByIndex(pointsToAssess[index].index, false);

            // Remove the point from the list so its not picked again
            pointsToAssess.RemoveAt(index);
        }
    }

    // Decimate the mesh, from a 0% active threshold that increases to 100% active at the end of the x-axis.
    public static void DecimateGradient(KoreMesh mesh)
    {
        // Get the number of points in the x axis, then determine the percentage barrier of the RNG we
        // need to pass to keep the point active.
        float xAxisActiveIncrement = 1f / (float)mesh.NumX;

        // create the random number generator
        Random random = new Random();

        for (int x = 0; x < mesh.NumX; x++)
        {
            float percentActiveThreshold = x * xAxisActiveIncrement;
            //Console.WriteLine($"X: {x}, Percent Active Threshold: {percentActiveThreshold}");

            for (int y = 0; y < mesh.NumY; y++)
            {
                // Generate a random number between 0 and 1
                float randomValue = (float)random.NextDouble();

                // Determine if the point should be active based on the gradient
                bool isActive = randomValue > percentActiveThreshold; // Compare against the gradient threshold

                // Set the point's active state in the mesh
                mesh.SetActiveByPos(x, y, isActive);
            }
        }
    }
}



