/*
 A point in d-dimensional space is defined as an d-tuple (x1, x2, ..., xd) where xi represents the i
th coordinate
of the point. For example, a point in 2-dimensional space is defined as (x1, x2) and often rewritten more
familiarly as (x, y).

Suppose that d-dimensional points are stored in a variation of the quadtree called the point quadtree.
When a new point is inserted, it is compared against the corresponding coordinates of the current point
to decide which one of 2d branches it should descend. For example, when a 2-dimensional point (x, y) is
compared against the current point (u, v) then it can descend one of four branches corresponding to one
of four possible cases.
 */
class Program {
    //An example, PointA = {1, 1} PointB = {2, 2}
    //                     x1, y1          x2, y2
    //
    //PointA is compared against PointB. PointA's x value is < PointB's, resulting in a 0 (Implicit else).
    //PointA's y value is also < PointB's, resulting in another 0.
    //So the final index result is 00 (in binary) which is 0. So PointA will go down PointB's 0 index.
    //
    //compares two d-dimensional points and maps the result to an index of B
    //B contains an array of nodes of size [0..2^d − 1]     d^2 = 4 nodes, d^3 = 8 nodes, etc..
    static int ComparePoints(int[] pointA, int[] pointB) {
        int index = 0;
        for (int i = 0; i < pointA.Length; i++) {
            index *= 2; // Shift bits left to make room for the next dimension's comparison
            if (pointA[i] >= pointB[i]) {
                index += 1; // Set the last bit to 1 if pointA's coordinate is greater than or equal to pointB's
            }
            // Implicit else: Bit remains 0 if pointA's coordinate is less than pointB's, no action needed
        }
        return index;
    }

    static void Main(string[] args) {
        // Example for 3-dimensional space
        int[] pointA = { 5, 10, 15 }; // Example point A
        int[] pointB = { 3, 10, 20 }; // Example point B to compare against
        int[] pointC = { 1, 1, 1 };

        int index = ComparePoints(pointA, pointB);
        Console.WriteLine("PointA: [" + pointA[0] + "," + pointA[1] + "," + pointA[2] + "]");
        Console.WriteLine("PointB: [" + pointB[0] + "," + pointB[1] + "," + pointB[2] + "]");
        Console.WriteLine($"Point A mapped to index: {index}");
        // Expected result explanation for a 3D example:
        // - For the first dimension, 5 >= 3, so bit is 1.
        // - For the second dimension, 10 >= 10, so bit is 1.
        // - For the third dimension, 15 < 20, so bit is 0.
        // Binary result: 110 (in base 2) which is 6 in decimal.

        index = ComparePoints(pointC, pointB);
        Console.WriteLine("");
        Console.WriteLine("PointC: [" + pointC[0] + "," + pointC[1] + "," + pointC[2] + "]");
        Console.WriteLine("PointB: [" + pointB[0] + "," + pointB[1] + "," + pointB[2] + "]");
        Console.WriteLine($"Point C mapped to index: {index}");

        //2-Dimensional example
        int[] pointD = {0, 0}; // Example point B to compare against
        int[] pointE = {1, 1};
        index = ComparePoints(pointD, pointE);
        Console.WriteLine("");
        Console.WriteLine("PointD: [" + pointD[0] + "," + pointD[1] + "]");
        Console.WriteLine("PointE: [" + pointE[0] + "," + pointE[1] + "]");
        Console.WriteLine($"Point D mapped to index: {index}");

        pointD[0] = 0;
        pointD[1] = 1;
        index = ComparePoints(pointD, pointE);
        Console.WriteLine("");
        Console.WriteLine("PointD: [" + pointD[0] + "," + pointD[1] + "]");
        Console.WriteLine("PointE: [" + pointE[0] + "," + pointE[1] + "]");
        Console.WriteLine($"Point D mapped to index: {index}");

        pointD[0] = 1;
        pointD[1] = 0;
        index = ComparePoints(pointD, pointE);
        Console.WriteLine("");
        Console.WriteLine("PointD: [" + pointD[0] + "," + pointD[1] + "]");
        Console.WriteLine("PointE: [" + pointE[0] + "," + pointE[1] + "]");
        Console.WriteLine($"Point D mapped to index: {index}");

        pointD[0] = 1;
        pointD[1] = 1;
        index = ComparePoints(pointD, pointE);
        Console.WriteLine("");
        Console.WriteLine("PointD: [" + pointD[0] + "," + pointD[1] + "]");
        Console.WriteLine("PointE: [" + pointE[0] + "," + pointE[1] + "]");
        Console.WriteLine($"Point D mapped to index: {index}");

        pointD[0] = 2;
        pointD[1] = 2;
        index = ComparePoints(pointD, pointE);
        Console.WriteLine("");
        Console.WriteLine("PointD: [" + pointD[0] + "," + pointD[1] + "]");
        Console.WriteLine("PointE: [" + pointE[0] + "," + pointE[1] + "]");
        Console.WriteLine($"Point D mapped to index: {index}");
    }
}
