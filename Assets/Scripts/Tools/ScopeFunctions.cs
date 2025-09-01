using System;

public static class ScopeFunctions
{
    public static T Let<T>(this T it, Action<T> action)
    {
        action(it);
        return it;
    }
    public static TReturn Run<T, TReturn>(this T it, Func<T, TReturn> action)
    {
        return action(it);
    }
}