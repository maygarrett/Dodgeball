#include "BaseState.h"
#include "BaseTransitionCondition.h"

FSM_State BaseState::TestTransitions()
{
    FSM_State returnState = FSM_Invalid;

    for (std::vector<BaseTransitionCondition*>::const_iterator it = m_transitions.begin();
        it != m_transitions.end();
        it++)
    {
        BaseTransitionCondition* transition = (*it);
        FSM_State transitionResult = transition->TestCondition();
        if (transitionResult != FSM_Invalid)
        {
            returnState = transitionResult;
            break;
        }
    }

    return returnState;
}
