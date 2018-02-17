#pragma once

#include "StateDefinitions.h"
#include <vector>

class BaseTransitionCondition;

class AIAgent;

class BaseState
{
public:
    BaseState(AIAgent* owner) : m_owner(owner){}

    virtual void Init() = 0;
    virtual void Shutdown() = 0;

    virtual void OnEnter() = 0;
    virtual void OnExit() = 0;
    virtual void Update(float deltaTime) = 0;
    virtual FSM_State GetStateName() const = 0;
    virtual void DebugDisplay() const = 0;

    virtual FSM_State TestTransitions();
    
protected:
    std::vector<BaseTransitionCondition*> m_transitions;
    AIAgent* m_owner;
};

