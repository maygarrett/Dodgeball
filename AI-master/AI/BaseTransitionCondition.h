#pragma once

#include "StateDefinitions.h"

class AIAgent;

class BaseTransitionCondition
{
public:
    BaseTransitionCondition(AIAgent* owner) : m_owner(owner) {}
    virtual FSM_State TestCondition() = 0;
protected:
    AIAgent *m_owner;
};
