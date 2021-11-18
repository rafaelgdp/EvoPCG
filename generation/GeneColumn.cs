using System;
using Godot;

public class GeneColumn
{
    private static long uid = 0;
    private long myuid = uid++;
    int groundHeight = 0;
    public int GroundHeight
    {
        get { return groundHeight; }
        set { groundHeight = Mathf.Clamp(value, 0, 12); }
    }

    public int ObstacleHeight
    {
        get
        {
            if (GroundHeight == 0)
            {
                return 0;
            }
            else
            {
                return GroundHeight + (HasSpike ? 1 : 0);
            }
        }
    }

    public bool IsSafe
    {
        get
        {
            if (HasSpike || GroundHeight == 0) return false;
            return true;
        }
    }
    private bool hasSpike = false;
    public bool HasSpike
    {
        get
        {
            if (GroundHeight == 0) return false;
            return hasSpike;
        }
        set
        {
            if (GroundHeight == 0 && value == !HasSpike)
            {
                // Possibly trying to mutate with masked HasSpike
                // So I force an internal toggle
                hasSpike = !hasSpike;
            }
            else
            {
                hasSpike = value;
            }
        }
    }
    private bool hasClock = false;
    public bool HasClock
    {
        get
        {
            if (GroundHeight == 0) return false;
            return hasClock;
        }
        set
        {
            if (GroundHeight == 0 && value == !HasClock)
            {
                // Possibly trying to mutate with masked HasSpike
                // So I force an internal toggle
                hasClock = !hasClock;
            }
            else
            {
                hasClock = value;
            }
        }
    }
    private float clockExtraTime = 5F;
    public float ClockExtraTime
    {
        set { clockExtraTime = Mathf.Clamp(value, 1F, 10F); }
        get
        {
            if (HasClock)
            {
                return clockExtraTime;
            }
            else
            {
                return 0F;
            }
        }
    }
    private int idealClockPosition
    {
        get
        {
            if (Previous == null || Next == null || (Previous.ObstacleHeight < ObstacleHeight && Next.ObstacleHeight < ObstacleHeight))
                return ObstacleHeight;
            return Mathf.Max(Previous.ObstacleHeight, Next.ObstacleHeight);
        }
    }

    public bool IsMutable = true;

    public int GlobalX = 0;

    public GeneColumn Next = null;
    public GeneColumn Previous = null;

    private static Random random = new Random();

    public GeneColumn(GeneColumn toBeCopied)
    {
        this.Clone(toBeCopied);
    }

    public GeneColumn(GeneColumn toBeCopied, GeneColumn previous, GeneColumn next) : this(toBeCopied)
    {
        this.Previous = previous;
        this.Next = next;
    }

    public GeneColumn(int globalX, GeneColumn previous, GeneColumn next) : this(
        previous, next, globalX,
        random.Next(0, 4), // GroundHeight
        random.Next(0, 2) == 0 ? false : true, // HasSpike
        random.Next(0, 2) == 0 ? false : true, // HasClock
        true // is mutable
    )
    { }

    public GeneColumn(GeneColumn left, GeneColumn right, int globalX, int groundHeight, bool hasSpike, bool hasClock, bool isMutable)
    {
        this.Previous = left;
        this.Next = right;
        this.GlobalX = globalX;
        this.GroundHeight = groundHeight;
        this.IsMutable = isMutable;
    }

    public bool ClockPlaced = false;

    internal char CellAtY(int y)
    {
        y = Mathf.Abs(y);
        if (y < GroundHeight) return 'G';
        if (HasSpike && y == GroundHeight) return 'S';
        if (HasClock && y == idealClockPosition)
            return ClockPlaced ? 'c' : 'C';
        return 'B';
    }

    public void Clone(GeneColumn gc)
    {
        this.GroundHeight = gc.GroundHeight;
        this.HasSpike = gc.HasSpike;
        this.HasClock = gc.HasClock;
        this.ClockPlaced = gc.ClockPlaced;
        this.GlobalX = gc.GlobalX;
        this.ClockExtraTime = gc.ClockExtraTime;
        this.IsMutable = gc.IsMutable;
    }

    public GeneColumn SeekNthColumn(int n)
    {
        if (n == 0) return this;
        int totalSteps = Mathf.Abs(n);
        GeneColumn iterator = this;
        if (n < 0)
        {
            for (int i = 0; i < totalSteps; i++)
            {
                iterator = iterator.Previous;
                if (iterator == null)
                {
                    return null;
                }
            }
            return iterator;
        }
        else
        {
            for (int i = 0; i < totalSteps; i++)
            {
                iterator = iterator.Next;
                if (iterator == null)
                {
                    return null;
                }
            }
            return iterator;
        }
    }

    public override string ToString()
    {
        String r = $"gX: {GlobalX}, uid: {myuid}, GroundHeight: {GroundHeight}, HasSpike: {HasSpike}, HasClock: {HasClock}, ClockExtraTime: {ClockExtraTime}.";
        return r;
    }

    public GeneColumn PreviousSafe()
    {
        GeneColumn iterator = Previous;
        while (iterator != null)
        {
            if (iterator.IsSafe) return iterator;
            iterator = iterator.Previous;
        }
        return null;
    }

    public GeneColumn NextSafe()
    {
        GeneColumn iterator = Next;
        while (iterator != null)
        {
            if (iterator.IsSafe) return iterator;
            iterator = iterator.Next;
        }
        return null;
    }

    public string ToCondensedString()
    {
        var r = $"|GH:{GroundHeight},S:";
        r += HasSpike ? "1" : "0";
        r += ",C:";
        r += HasClock ? "1" : "0";
        r += $",CT:{ClockExtraTime}|";
        return r;
    }
}