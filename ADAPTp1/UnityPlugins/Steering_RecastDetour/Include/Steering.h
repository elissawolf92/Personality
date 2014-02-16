#ifndef NAVIGATIONMANAGER_H
#define NAVIGATIONMANAGER_H

#include <DetourNavMesh.h>
#include <DetourCrowd.h>
#include <DetourNavMeshQuery.h>

struct Vector3
{
	float x, y, z;
};

enum NavigationQuality 
{
	NAVIGATIONQUALITY_LOW,
	NAVIGATIONQUALITY_MED,
	NAVIGATIONQUALITY_HIGH
};

enum Pushiness 
{
	PUSHINESS_LOW,
	PUSHINESS_MEDIUM,
	PUSHINESS_HIGH
};

class SteeringManager
{
public:
	bool init(unsigned char* navMeshData, int navMeshDataSize, int maxAgents, float maxAgentRadius);
	void update(float dT);

	int addAgent(Vector3 pos, float radius, float height, float accel, float maxSpeed);
	void removeAgent(int agent);

	void updateAgentNavigationQuality(int agent, NavigationQuality nq);
	void updateAgentPushiness(int agent, Pushiness pushiness);
	void updateAgentMaxSpeed(int agent, float maxSpeed);
	void updateAgentMaxAcceleration(int agent, float accel);

	void setAgentTarget(int agent, Vector3 target);
	void setAgentMobile(int agent, bool mobile);

	Vector3 getAgentPosition(int agent);
	Vector3 getAgentCurrentVelocity(int agent);
	Vector3 getAgentDesiredVelocity(int agent);

	Vector3 getClosestWalkablePosition(Vector3 pos);

private:
	dtNavMesh navMesh;
	dtNavMeshQuery query;
	dtCrowd crowd;

	bool initNavMesh(unsigned char* navmeshData, int navmeshDataSize);
	bool initQuery();
	bool initCrowd(int maxAgents, float maxAgentRadius);
};

#endif