#pragma once

#include "BaseTransitionCondition.h"

class DeathTransitionCondition : public BaseTransitionCondition
{
public:
    DeathTransitionCondition(AIAgent* owner) : BaseTransitionCondition(owner) {}
    FSM_State TestCondition() override; 
};
