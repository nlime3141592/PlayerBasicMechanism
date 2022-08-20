using System;

public class DiscreteParabolaGraph
{
    public int length { get; private set; }

    public float this[int index]
    {
        get => m_values[index];
    }

    private float[] m_values;

    public DiscreteParabolaGraph(int length)
    {
        int i;
        int l = length - 1;
        float x;

        this.length = length;
        m_values = new float[length];

        for(i = 0; i <= l; i++)
        {
            x = i;
            m_values[i] = x * (2 * l - x) / (l * l);
        }
    }
}