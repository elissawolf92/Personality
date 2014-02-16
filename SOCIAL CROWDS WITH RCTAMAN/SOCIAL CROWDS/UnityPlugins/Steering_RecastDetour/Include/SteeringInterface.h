#include "Steering.h"
#define EXPORT extern "C" __declspec(dllexport)

EXPORT SteeringManager* createSteeringManager() 
{ 
	return new SteeringManager; 
}

EXPORT void destroySteeringManager(SteeringManager* manager) 
{ 
	delete manager; 
}

// Everything below here just exposes the public class functionality
EXPORT bool init(SteeringManager* manager,unsigned char* navMeshData, 
	int navMeshDataSize, int maxAgents, float maxAgentRadius)
{
	return manager->init(navMeshData, navMeshDataSize, maxAgents, maxAgentRadius);
}

EXPORT void update(SteeringManager* manager, float dT)
{
	manager->update(dT);
}

EXPORT int addAgent(SteeringManager* manager, Vector3 pos, 
	float radius, float height, float accel, float maxSpeed)
{
	return manager->addAgent(pos, radius, height, accel, maxSpeed);
}

EXPORT void removeAgent(SteeringManager* manager, int agent)
{
	manager->removeAgent(agent);
}

EXPORT void updateAgentNavigationQuality(
	SteeringManager* manager, int agent, NavigationQuality nq)
{
	manager->updateAgentNavigationQuality(agent, nq);
}

EXPORT void updateAgentPushiness(
	SteeringManager* manager, int agent, Pushiness pushiness)
{
	manager->updateAgentPushiness(agent, pushiness);
}

EXPORT void updateAgentMaxSpeed(
	SteeringManager* manager, int agent, float speed)
{
	manager->updateAgentMaxSpeed(agent, speed);
}

EXPORT void updateAgentMaxAcceleration(
	SteeringManager* manager, int agent, float accel)
{
	manager->updateAgentMaxAcceleration(agent, accel);
}

EXPORT void setAgentTarget(
	SteeringManager* manager, int agent, Vector3 pos)
{
	manager->setAgentTarget(agent, pos);
}

EXPORT void setAgentMobile(
	SteeringManager* manager, int agent, bool mobile)
{
	manager->setAgentMobile(agent, mobile);
}

EXPORT Vector3 getAgentPosition(
	SteeringManager* manager, const int agent)
{
	return manager->getAgentPosition(agent);
}

EXPORT Vector3 getAgentCurrentVelocity(
	SteeringManager* manager, const int agent)
{
	return manager->getAgentCurrentVelocity(agent);
}

EXPORT Vector3 getAgentDesiredVelocity(
	SteeringManager* manager, const int agent)
{
	return manager->getAgentDesiredVelocity(agent);
}

EXPORT Vector3 getClosestWalkablePosition(
	SteeringManager* manager, Vector3 pos)
{
	return manager->getClosestWalkablePosition(pos);
}