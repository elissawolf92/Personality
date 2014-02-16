#ifndef NAVMESH_H
#define NAVMESH_H

#include <Recast.h>
#include <DetourNavMesh.h>
#include <DetourNavMeshQuery.h>

extern float* providedVertices;
extern int* providedIndices;
extern int numProvidedVertices;
extern int numProvidedIndices;

extern rcHeightfield* solid;
extern unsigned char* triAreas;
extern rcCompactHeightfield* chf;
extern rcContourSet* cset;
extern rcPolyMesh* pmesh;
extern rcPolyMeshDetail* dmesh;

#define EXPORT extern "C" __declspec(dllexport)

EXPORT dtNavMesh* DebugInitNavmesh(unsigned char* data, int dataSize);
EXPORT void DebugDestroyNavmesh(dtNavMesh* nm);

EXPORT void RetrieveNavmeshData(unsigned char* buffer);
EXPORT int BuildNavmesh(
	int numVertices,
	float* vertices,
	int numIndices,
	int* indices,
	float minX,
	float minY,
	float minZ,
	float maxX,
	float maxY,
	float maxZ,
	float cellSize,
	float cellHeight,
	float walkableHeight,
	float walkableSlopeAngle,
	float walkableClimb,
	float walkableRadius,
	float maxEdgeLen,
	float maxSimplificationError,
	bool monotonePartitioning,
	float minRegionArea,
	float mergeRegionArea,
	float detailSampleDist,
	float detailSampleMaxError,
	bool keepIntermediate,
	int oneMillion);

#endif