#include "AttackState.h"
#include <iostream>

AttackState::AttackState(AIAgent* owner) : BaseState(owner), timeElapsed(0.0f)
{
    std::cout << "AttackState()" << std::endl;
}

void AttackState::Init()
{
    std::cout << "AttackState::Init()" << std::endl;
}

void AttackState::Shutdown()
{
    std::cout << "AttackState::Shutdown()" << std::endl;
}

void AttackState::OnEnter()
{
    std::cout << "AttackState::OnEnter()" << std::endl;
    timeElapsed = 0.0f;
}

void AttackState::OnExit()
{
    std::cout << "AttackState::OnExit()" << std::endl;
}

void AttackState::Update(float deltaTime)
{
    timeElapsed += deltaTime;
    //std::cout << "time elapsed " << std::fixed << std::setprecision(3) << timeElapsed << std::endl;
}
