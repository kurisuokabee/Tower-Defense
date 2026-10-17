using UnityEngine;

public static class Ease
{
    public static float EaseOutQuad(float t) => 1-(1-t)*(1-t);   
    public static float EaseInOutQuad(float t) => t < 0.5f ? 2*t*t : 1-(-2*t+2)*(-2*t+2)/2;
    public static float EaseInOutSine(float t) => -(Mathf.Cos(Mathf.PI*t)-1)/2;
}