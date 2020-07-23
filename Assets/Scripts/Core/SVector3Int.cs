using UnityEngine;
 using System;
 using System.Collections;
 
 /// <summary>
 /// Since unity doesn't flag theInt Vector3 as serializable, we
 /// need to create our own version. This one will automatically convert
 /// betweenInt Vector3 and SerializableVector3Int
 /// </summary>
 [System.Serializable]
 public struct SVector3Int
 {
    public static SVector3Int zero = new SVector3Int(0, 0, 0);

    /// <summary>
    /// x component
    /// </summary>
    public int x;
     
     /// <summary>
     /// y component
     /// </summary>
     public int y;
     
     /// <summary>
     /// z component
     /// </summary>
     public int z;
     
     /// <summary>
     /// Constructor
     /// </summary>
     /// <param name="rX"></param>
     /// <param name="rY"></param>
     /// <param name="rZ"></param>
     public SVector3Int(int rX, int rY, int rZ)
     {
         x = rX;
         y = rY;
         z = rZ;
     }
     
     /// <summary>
     /// Returns a string representation of the object
     /// </summary>
     /// <returns></returns>
     public override string ToString()
     {
         return String.Format("[{0}, {1}, {2}]", x, y, z);
     }
     
     /// <summary>
     /// Automatic conversion from SerializableVector3Int toInt Vector3
     /// </summary>
     /// <param name="rValue"></param>
     /// <returns></returns>
     public static implicit operator Vector3Int(SVector3Int rValue)
     {
         return new Vector3Int(rValue.x, rValue.y, rValue.z);
     }
     
     /// <summary>
     /// Automatic conversion fromInt Vector3 to SerializableVector3Int
     /// </summary>
     /// <param name="rValue"></param>
     /// <returns></returns>
     public static implicit operator SVector3Int(Vector3Int rValue)
     {
         return new SVector3Int(rValue.x, rValue.y, rValue.z);
     }

     public static implicit operator SVector3Int(Vector3 rValue)
     {
         return new SVector3Int((int) rValue.x, (int) rValue.y, (int) rValue.z);
     }
 }