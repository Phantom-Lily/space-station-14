using System.Collections.Generic;
using System.Threading.Tasks;
using Content.Server.Storage.Components;
using Content.Shared.Construction;
using Content.Shared.Examine;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public class Locked : IGraphCondition
    {
        [DataField("locked")]
        public bool IsLocked { get; private set; } = true;

        public bool Condition(EntityUid uid, IEntityManager entityManager)
        {
            if (!entityManager.TryGetComponent(uid, out LockComponent? lockcomp))
                return false;

            return lockcomp.Locked == IsLocked;
        }

        public bool DoExamine(ExaminedEvent args)
        {
            var entity = args.Examined;

            if (!entity.TryGetComponent(out LockComponent? lockcomp)) return false;

            switch (IsLocked)
            {
                case true when !lockcomp.Locked:
                    args.PushMarkup(Loc.GetString("construction-examine-condition-lock"));
                    return true;
                case false when lockcomp.Locked:
                    args.PushMarkup(Loc.GetString("construction-examine-condition-unlock"));
                    return true;
            }

            return false;
        }

        public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
        {
            yield return new ConstructionGuideEntry()
            {
                Localization = IsLocked
                    ? "construction-step-condition-wire-panel-lock"
                    : "construction-step-condition-wire-panel-unlock"
            };
        }
    }
}
