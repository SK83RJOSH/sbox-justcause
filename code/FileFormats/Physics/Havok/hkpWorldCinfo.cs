namespace JustCause.FileFormats.Physics.Havok;

using JustCause.FileFormats.Utilities;
using System.IO;

public enum hkpSimulationType : sbyte
{
	Invalid,
	Discrete,
	Continuous,
	MultiThreaded,
}

public enum hkpContactPointGeneration : sbyte
{
	AcceptAlways,
	RejectDubious,
	RejectMany,
}

public enum hkpBroadPhaseBorderBehaviour : sbyte
{
	Assert,
	FixEntity,
	RemoveEntity,
	DoNothing,
}

public class hkpWorldCinfo : hkReferencedObject
{
	public hkVector4 Gravity;
	public int BroadPhaseQuerySize;
	public float ContactRestingVelocity;
	public hkpBroadPhaseBorderBehaviour BroadPhaseBorderBehaviour;
	public hkAabb BroadPhaseWorldAabb;
	public float CollisionTolerance;
	public hkpCollisionFilter CollisionFilter;
	public hkpConvexListFilter ConvexListFilter;
	public float ExpectedMaxLinearVelocity;
	public int SizeOfToiEventQueue;
	public float ExpectedMinPsiDeltaTime;
	public hkWorldMemoryAvailableWatchDog EmoryWatchDog;
	public int BroadPhaseNumMarkers;
	public hkpContactPointGeneration ContactPointGeneration;
	public float SolverTau;
	public float SolverDamp;
	public int SolverIterations;
	public int SolverMicrosteps;
	public bool ForceCoherentConstraintOrderingInSolver;
	public float SnapCollisionToConvexEdgeThreshold;
	public float SnapCollisionToConcaveEdgeThreshold;
	public bool EnableToiWeldRejection;
	public bool EnableDeprecatedWelding;
	public float IterativeLinearCastEarlyOutDistance;
	public int IterativeLinearCastMaxIterations;
	public float HighFrequencyDeactivationPeriod;
	public float LowFrequencyDeactivationPeriod;
	public byte DeactivationNumInactiveFramesSelectFlag0;
	public byte DeactivationNumInactiveFramesSelectFlag1;
	public byte DeactivationIntegrateCounter;
	public bool ShouldActivateOnRigidBodyTransformChange;
	public float DeactivationReferenceDistance;
	public float ToiCollisionResponseRotateNormal;
	public int AxSectorsPerCollideTask;
	public bool ProcessToisMultithreaded;
	public int AxEntriesPerToiCollideTask;
	public bool EnableDeactivation;
	public hkpSimulationType SimulationType;
	public bool EnableSimulationIslands;
	public uint InDesiredIslandSize;
	public bool ProcessActionsInSingleThread;
	public float FrameMarkerPsiSnap;

	public new bool Read(BinaryReader reader, Endian endian)
	{
		if (!base.Read(reader, endian))
		{
			return false;
		}

		if (!reader.Read(out Gravity, endian))
		{
			return false;
		}

		if (!reader.Read(out BroadPhaseQuerySize, endian))
		{
			return false;
		}

		if (!reader.Read(out ContactRestingVelocity, endian))
		{
			return false;
		}

		if (!reader.Read(out BroadPhaseBorderBehaviour, endian))
		{
			return false;
		}

		if (!reader.Read(out BroadPhaseWorldAabb, endian))
		{
			return false;
		}

		if (!reader.Read(out CollisionTolerance, endian))
		{
			return false;
		}

		int pointer;
		long position;

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out CollisionFilter, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out ConvexListFilter, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out ExpectedMaxLinearVelocity, endian))
		{
			return false;
		}

		if (!reader.Read(out SizeOfToiEventQueue, endian))
		{
			return false;
		}

		if (!reader.Read(out ExpectedMinPsiDeltaTime, endian))
		{
			return false;
		}

		if (!reader.Read(out pointer, endian))
		{
			return false;
		}

		position = reader.Tell();
		reader.Seek(pointer);

		if (pointer != 0 && !reader.Read(out EmoryWatchDog, endian))
		{
			return false;
		}

		reader.Seek(position);

		if (!reader.Read(out BroadPhaseNumMarkers, endian))
		{
			return false;
		}

		if (!reader.Read(out ContactPointGeneration, endian))
		{
			return false;
		}

		if (!reader.Read(out SolverTau, endian))
		{
			return false;
		}

		if (!reader.Read(out SolverDamp, endian))
		{
			return false;
		}

		if (!reader.Read(out SolverIterations, endian))
		{
			return false;
		}

		if (!reader.Read(out SolverMicrosteps, endian))
		{
			return false;
		}

		if (!reader.Read(out ForceCoherentConstraintOrderingInSolver, endian))
		{
			return false;
		}

		if (!reader.Read(out SnapCollisionToConvexEdgeThreshold, endian))
		{
			return false;
		}

		if (!reader.Read(out SnapCollisionToConcaveEdgeThreshold, endian))
		{
			return false;
		}

		if (!reader.Read(out EnableToiWeldRejection, endian))
		{
			return false;
		}

		if (!reader.Read(out EnableDeprecatedWelding, endian))
		{
			return false;
		}

		if (!reader.Read(out IterativeLinearCastEarlyOutDistance, endian))
		{
			return false;
		}

		if (!reader.Read(out IterativeLinearCastMaxIterations, endian))
		{
			return false;
		}

		if (!reader.Read(out HighFrequencyDeactivationPeriod, endian))
		{
			return false;
		}

		if (!reader.Read(out LowFrequencyDeactivationPeriod, endian))
		{
			return false;
		}

		if (!reader.Read(out DeactivationNumInactiveFramesSelectFlag0, endian))
		{
			return false;
		}

		if (!reader.Read(out DeactivationNumInactiveFramesSelectFlag1, endian))
		{
			return false;
		}

		if (!reader.Read(out DeactivationIntegrateCounter, endian))
		{
			return false;
		}

		if (!reader.Read(out ShouldActivateOnRigidBodyTransformChange, endian))
		{
			return false;
		}

		if (!reader.Read(out DeactivationReferenceDistance, endian))
		{
			return false;
		}

		if (!reader.Read(out ToiCollisionResponseRotateNormal, endian))
		{
			return false;
		}

		if (!reader.Read(out AxSectorsPerCollideTask, endian))
		{
			return false;
		}

		if (!reader.Read(out ProcessToisMultithreaded, endian))
		{
			return false;
		}

		if (!reader.Read(out AxEntriesPerToiCollideTask, endian))
		{
			return false;
		}

		if (!reader.Read(out EnableDeactivation, endian))
		{
			return false;
		}

		if (!reader.Read(out SimulationType, endian))
		{
			return false;
		}

		if (!reader.Read(out EnableSimulationIslands, endian))
		{
			return false;
		}

		if (!reader.Read(out InDesiredIslandSize, endian))
		{
			return false;
		}

		if (!reader.Read(out ProcessActionsInSingleThread, endian))
		{
			return false;
		}

		if (!reader.Read(out FrameMarkerPsiSnap, endian))
		{
			return false;
		}

		reader.Move(34);
		return true;
	}

	public static bool Read(BinaryReader reader, out hkpWorldCinfo value, Endian endian)
	{
		value = new();
		return value.Read(reader, endian);
	}
}
