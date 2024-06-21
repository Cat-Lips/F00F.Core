using System;
using Godot;

namespace F00F;

public abstract partial class SceneState<TScene, TState> : Node
    where TScene : Node
    where TState : struct, Enum
{
    public TScene Context { get; init; }

    private Action<TState, Action<SceneState<TScene, TState>>> NewState;

    protected void SetNextState(TState state)
        => NewState?.Invoke(state, null);

    protected void SetNextState<TNext>(TState state, Action<TNext> init) where TNext : SceneState<TScene, TState>
        => NewState?.Invoke(state, x => init((TNext)x));

    protected static void Init(TScene context, TState initial,
        Func<TState, SceneState<TScene, TState>> CreateNewState,
        Action<SceneState<TScene, TState>> init = null)
        => Init(context, initial, CreateNewState, out var _, out var _, init);

    protected static void Init(TScene context, TState initial,
        Func<TState, SceneState<TScene, TState>> CreateNewState,
        out Func<SceneState<TScene, TState>> GetState,
        out Action<TState, Action<SceneState<TScene, TState>>> SetState,
        Action<SceneState<TScene, TState>> init = null)
    {
        SceneState<TScene, TState> currentState = null;
        AssignState(initial, init);
        GetState = () => currentState;
        SetState = SwitchState;

        void SwitchState(TState state, Action<SceneState<TScene, TState>> init)
        {
            ClearState();
            AssignState(state, init);
        }

        void ClearState()
        {
            currentState.NewState -= SwitchState;
            context.RemoveChild(currentState, free: true);
        }

        void AssignState(TState state, Action<SceneState<TScene, TState>> init)
        {
            currentState = CreateNewState(state);
            currentState.NewState += SwitchState;
            currentState.Name = $"{typeof(TScene).Name}State: {state}";

            init?.Invoke(currentState);

            context.AddChild(currentState, owner: Editor.IsEditor ? context : null);
        }
    }
}
