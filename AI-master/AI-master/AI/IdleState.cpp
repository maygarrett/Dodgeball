#include "IdleState.h"
#include "DeathTransitionCondition.h"
#include <iostream>

IdleState::IdleState(AIAgent* owner) : BaseState(owner), timeElapsed(0.0f)
{
    std::cout << "IdleState()" << std::endl;
    m_transitions.push_back(new DeathTransitionCondition(owner));
}

void IdleState::Init() 
{
    std::cout << "IdleState::Init()" << std::endl;
}

void IdleState::Shutdown() 
{
    std::cout << "IdleState::Shutdown()" << std::endl;
}

void IdleState::OnEnter() 
{
    std::cout << "IdleState::OnEnter()" << std::endl;
    timeElapsed = 0.0f;
}

void IdleState::OnExit() 
{
    std::cout << "IdleState::OnExit()" << std::endl;
}

void IdleState::Update(float deltaTime) 
{
    timeElapsed += deltaTime;
    //std::cout << "time elapsed " << std::fixed << std::setprecision(3) << timeElapsed << std::endl;
}
