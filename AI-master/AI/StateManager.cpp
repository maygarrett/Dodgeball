#include "StateManager.h"
#include <iostream>

StateManager::StateManager(AIAgent* owner) :
    m_currentState(nullptr)
    , m_desiredState(FSM_Invalid)
    , m_owner(owner)
{

}

void StateManager::Update(float deltaTime)
{
    FSM_State currentState = m_currentState ? m_currentState->GetStateName() : FSM_Invalid;
    if (currentState != m_desiredState && m_desiredState != FSM_Invalid)
    {
        BaseState* desiredState = GetState(m_desiredState);
        if (desiredState)
        {
            if (m_currentState)
            {
                m_currentState->OnExit();
            }

            desiredState->OnEnter();
            m_currentState = desiredState;
        }
    }

    if (m_currentState)
    {
        m_currentState->Update(deltaTime);
        m_desiredState = m_currentState->TestTransitions();
    }
}

BaseState* StateManager::GetState(FSM_State stateName) const
{
    for (std::vector<BaseState*>::const_iterator it = m_states.begin();
        it != m_states.end();
        it++)
    {
        BaseState* state = (*it);
        if (state->GetStateName() == stateName)
        {
            return state;
        }
    }

        return nullptr;
}

void StateManager::AddState(BaseState* state)
{
    m_states.push_back(state);
    std::cout << "Added State" << std::endl;
}

void StateManager::RemoveState(BaseState* removeState)
{
    for(std::vector<BaseState*>::iterator it = m_states.begin();
        it != m_states.end();
        it++)

        if (removeState == (*it))
        {
            m_states.erase(it);
            return;
        }
}