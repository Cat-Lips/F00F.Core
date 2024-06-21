using System;
using Godot;
using Godot.Collections;
using static Godot.RenderingServer;

namespace F00F;

public abstract class GlobalShaderParam<T, [MustBeVariant] TValue>(TValue defaultValue = default) where T : new()
{
    public static T Instance => field ??= new();

    protected abstract string ShaderType { get; }
    protected virtual string ParamName { get; } = typeof(T).Name.ToSnakeCase();
    public TValue Value { get; set => this.Set(ref field, value, OnValueChanged, ValueChanged); } = defaultValue;
    public event Action ValueChanged;

    public void Add()
    {
        ProjectSettings.SetSetting($"shader_globals/{ParamName}", new Dictionary()
        {
            { "type", ShaderType },
            { "value", Variant.From(Value) }
        });
    }

    public void Remove()
        => ProjectSettings.SetSetting($"shader_globals/{ParamName}", default);

    public bool IsSet()
        => ProjectSettings.HasSetting($"shader_globals/{ParamName}");

    public TValue GetDefault()
        => ProjectSettings.GetSetting($"shader_globals/{ParamName}").AsGodotDictionary()["value"].As<TValue>();

    private void OnValueChanged()
        => GlobalShaderParameterSet(ParamName, Variant.From(Value));
}
