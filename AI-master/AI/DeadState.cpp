#include "DeadState.h"
#include <iostream>

DeadState::DeadState(AIAgent* owner) : BaseState(owner), timeElapsed(0.0f)
{
    std::cout << "DeadState()" << std::endl;
}

void DeadState::Init()
{
    std::cout << "DeadState::Init()" << std::endl;
}

void DeadState::Shutdown()
{
    std::cout << "DeadState::Shutdown()" << std::endl;
}

void DeadState::OnEnter()
{
    std::cout << "DeadState::OnEnter()" << std::endl;
    timeElapsed = 0.0f;
}

void DeadState::OnExit()
{
    std::cout << "DeadState::OnExit()" << std::endl;
}

void DeadState::Update(float deltaTime)
{
    timeElapsed += deltaTime;
    //std::cout << "time elapsed " << std::fixed << std::setprecision(3) << timeElapsed << std::endl;
}
