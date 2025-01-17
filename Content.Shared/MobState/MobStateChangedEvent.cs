﻿using Content.Shared.MobState.Components;
using Content.Shared.MobState.State;
using Robust.Shared.GameObjects;

namespace Content.Shared.MobState
{
    public class MobStateChangedEvent : EntityEventArgs
    {
        public MobStateChangedEvent(
            MobStateComponent component,
            IMobState? oldMobState,
            IMobState currentMobState)
        {
            Component = component;
            OldMobState = oldMobState;
            CurrentMobState = currentMobState;
        }

        public IEntity Entity => Component.Owner;

        public MobStateComponent Component { get; }

        public IMobState? OldMobState { get; }

        public IMobState CurrentMobState { get; }
    }
}
