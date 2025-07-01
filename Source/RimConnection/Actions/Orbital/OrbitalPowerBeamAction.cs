using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RimWorld;
using Verse;

namespace RimConnection
{
    public class OrbitalPowerBeamAction: Action, IAction
    {
        private int numberToSpawn = 5;
        public OrbitalPowerBeamAction()
        {
            Name = "Orbital Power Beam";
            Description = "A destructive beam of heat";
            Category = "Orbital";
            Prefix = "Trigger";
        }

        public override void Execute(int amount, string boughtBy)
        {
            Map currentMap = Find.CurrentMap;

            var colonists = Find.ColonistBar.GetColonistsInOrder()
                .Where(colonist => !colonist.Dead).ToList();

            if (colonists.Count == 0)
            {
                return;
            }

            for (int i = 0; i < numberToSpawn; i++)
            {
                Pawn colonist = colonists.RandomElement();
                IntVec3 targetCell;
                Predicate<IntVec3> validator = (IntVec3 c) => c.InBounds(currentMap) && c.Standable(currentMap);
                if (!CellFinder.TryFindRandomCellNear(colonist.Position, currentMap, 3, validator, out targetCell))
                {
                    targetCell = colonist.Position;
                }

                PowerBeam powerBeam = (PowerBeam)GenSpawn.Spawn(ThingDefOf.PowerBeam, targetCell, currentMap);
                powerBeam.duration = 600;
                powerBeam.StartStrike();
            }

            AlertManager.BadEventNotification("{0} requested a bombardment from space!", colonists[0].Position, boughtBy);

        }
    }
}