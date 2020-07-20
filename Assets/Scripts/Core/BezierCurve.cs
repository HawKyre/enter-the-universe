using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve {

    private List<Vector2> _controlPoints;
    private Dictionary<double, Vector2> _pointBuffer;
    private int _degree;

    // Degree can't belarger than 6
    public BezierCurve(List<Vector2> points)
    {
        if (points.Count > 6)
        {
            throw new ArgumentOutOfRangeException();
        }

        _degree = points.Count;
        _controlPoints = points;

        _pointBuffer = new Dictionary<double, Vector2>();
    }

    // Implementation as seen here
    // https://stackoverflow.com/questions/41663348/bezier-curve-of-n-order
    public Vector2 GetPoint(double t)
    {
        double nt = 1 - t;
        
        // check for the buffer
        if (_pointBuffer.ContainsKey(t))
        {
            Debug.Log("Saved!!");
            Vector2 point;
            _pointBuffer.TryGetValue(t, out point);
            return point;
        }
        

        // Copy the points to a new list
        List<Vector2> points = new List<Vector2>(_degree);
        for (int i = 0; i < _degree; i++)
        {
            points.Add(_controlPoints[i]);
        }

        while (points.Count > 1)
        {
            for (int i = 0, e = points.Count - 1; i < e; i++)
            {
                points[i] = (float) nt * points[i] + (float) t * points[i + 1];
            }
            points.RemoveAt(points.Count - 1);
        }

        _pointBuffer.Add(t, points[0]);

        return points[0];
    }
}