[System.Serializable]
public class Range
{
    private int min;
    private int max;

    public Range(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public int GetInRange()
    {
        if (min == max)
        {
            return min;
        }
        return UnityEngine.Random.Range(min, max+1);
    }
}