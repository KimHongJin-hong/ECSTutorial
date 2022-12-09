using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public readonly partial struct TurretAspect : IAspect
{
    public readonly RefRO<Turret> m_turret;
    public readonly RefRO<URPMaterialPropertyBaseColor> m_baseColor;

    public Entity CannonBallSpawn => m_turret.ValueRO.cannonBallSpawn; 
    public Entity CannonBallPrefab => m_turret.ValueRO.cannonBallPrefab;
    public float4 Color => m_baseColor.ValueRO.Value;
}
