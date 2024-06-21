using System;
using Godot;

namespace F00F;

public partial class Camera
{
    public event Action ShakeChanged;
    public float Shake { get; private set => this.Set(ref field, value, ShakeChanged); }

    private float _rumbleShake;
    public void SetRumbleShake(float intensity)
        => _rumbleShake = intensity.ClampMin(0);

    private float _impactShake;
    public void AddImpactShake(float intensity, float duration = 0, float decay = 1)
    {
        if (intensity <= 0) return;

        _impactShake += intensity;

        var tween = CreateTween()
            .SetPauseMode(Tween.TweenPauseMode.Process)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
        tween.TweenInterval(duration);
        tween.TweenMethod(DecayShake(), intensity, 0, decay);

        Callable DecayShake() => Callable.From<float>(x =>
        {
            var currentDecay = intensity - x;
            _impactShake = (_impactShake - currentDecay).ClampMin(0);
            intensity = x;
        });
    }

    #region Private

    private float _shakeStep;
    private void ApplyShake(float delta)
    {
        Shake = _rumbleShake + _impactShake;

        if (Shake is 0) return;
        if (!Config.ShakeEnabled) return;

        _shakeStep += delta * Config.ShakeRoughness;

        var shakeX = Config.ShakeNoise.GetNoise2D(_shakeStep, 100) * Shake * Config.ShakeScreenMultiplier;
        var shakeY = Config.ShakeNoise.GetNoise2D(_shakeStep, 200) * Shake * Config.ShakeScreenMultiplier;
        var shakeZ = Config.ShakeNoise.GetNoise2D(_shakeStep, 300) * Shake * Config.ShakeScreenMultiplier * .25f;
        var shakeYaw = Config.ShakeNoise.GetNoise2D(_shakeStep, 400) * Shake * Config.ShakeCameraMultiplier;
        var shakeRoll = Config.ShakeNoise.GetNoise2D(_shakeStep, 500) * Shake * Config.ShakeCameraMultiplier;
        var shakePitch = Config.ShakeNoise.GetNoise2D(_shakeStep, 600) * Shake * Config.ShakeCameraMultiplier;

        TranslateObjectLocal(new(shakeX, shakeY, shakeZ));
        GlobalBasis *= Basis.FromEuler(new(shakePitch, shakeYaw, shakeRoll));
    }

    #endregion
}
