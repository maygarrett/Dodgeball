#include "DeathTransitionCondition.h"
#include "AIAgent.h"

FSM_State DeathTransitionCondition::TestCondition() 
{ 
    return m_owner->IsDead() ? FSM_Dead : FSM_Invalid; 
}

