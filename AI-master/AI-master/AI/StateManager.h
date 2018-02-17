#pragma once
#include "BaseState.h"
#include <vector>

class AIAgent;

class StateManager
{
public:
    StateManager(AIAgent* owner);
    void SetDesiredState(FSM_State state) { m_desiredState = state; }

    void Update(float deltaTime);

    void AddState(BaseState* state);
    void RemoveState(BaseState* state);
    FSM_State GetCurrentState() const { return m_currentState->GetStateName(); }

    void DebugDisplay() const 
    { 
        if (m_currentState)
        {
            m_currentState->DebugDisplay();
        }
    }

protected:
    BaseState* GetState(FSM_State stateName) const;
    AIAgent* m_owner;
private:
    std::vector<BaseState*> m_states;
    FSM_State m_desiredState;
    BaseState* m_currentState;
};
