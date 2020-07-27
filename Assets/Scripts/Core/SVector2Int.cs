using UnityEngine;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

/// <summary>
/// Since unity doesn't flag the Vector3 as serializable, we
/// need to create our own version. This one will automatically convert
/// between Vector3 and SerializableVector3
/// </summary>
[System.Serializable]
[TypeConverter(typeof(SVector2IntConverter))]
public struct SVector2Int
{

    /// <summary>
    /// x component
    /// </summary>
    public int x;
    
    /// <summary>
    /// y component
    /// </summary>
    public int y;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SVector2Int(int rX, int rY)
    {
        x = rX;
        y = rY;
    }
    
    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}]", x, y);
    }
    
    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector2Int(SVector2Int rValue)
    {
        return new Vector2Int(rValue.x, rValue.y);
    }
    
    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SVector2Int(Vector2Int rValue)
    {
        return new SVector2Int(rValue.x, rValue.y);
    }
}

public class SVector2IntConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        var casted = value as string;
        if (casted != null)
        {
            string[] s = casted.Split(',');
            int x = int.Parse(s[0].Replace('[', ' '));
            int y = int.Parse(s[1].Replace(']', ' '));
            return new SVector2Int(x, y);
        }
        else
        {
            return base.ConvertFrom(context, culture, value);
        }
    }
}