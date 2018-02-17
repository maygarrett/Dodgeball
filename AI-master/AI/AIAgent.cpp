#include "AIAgent.h"
#include "StateManager.h"
#include "IdleState.h"
#include "AttackState.h"
#include "DeadState.h"

AIAgent::AIAgent() : m_health(100)
{
    m_stateManager = new StateManager(this);
    m_stateManager->AddState(new IdleState(this));
    m_stateManager->AddState(new AttackState(this));
    m_stateManager->AddState(new DeadState(this));
    m_stateManager->SetDesiredState(FSM_Idle);
}

AIAgent::~AIAgent()
{
    if (m_stateManager)
    {
        delete m_stateManager;
        m_stateManager = nullptr;
    }
}

void AIAgent::Update(float deltaTime)
{
    m_stateManager->Update(deltaTime);
}

void AIAgent::GoToIdle()
{
    m_stateManager->SetDesiredState(FSM_Idle);
}

void AIAgent::Attack(AIAgent* target)
{
    m_stateManager->SetDesiredState(FSM_Attack);
}

void AIAgent::Die()
{
    m_health = -10;
    //m_stateManager->SetDesiredState(FSM_Dead);
}

bool AIAgent::IsDead() const
{
    return m_health < 0;
}

void AIAgent::DebugDisplay() const
{
    if (m_stateManager)
    {
        m_stateManager->DebugDisplay();
    }
}
