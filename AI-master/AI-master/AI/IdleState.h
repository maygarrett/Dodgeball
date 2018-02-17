#pragma once

#include "BaseState.h"
#include <iostream>
#include <iomanip>
class IdleState : public BaseState
{
public:
    IdleState(AIAgent* owner);

    void Init() override;
    void Shutdown() override;

    void OnEnter() override;
    void OnExit() override;
    void Update(float deltaTime) override;

    FSM_State GetStateName() const override { return FSM_Idle; }

    void DebugDisplay() const override  
    { 
        std::cout << "Idle State" << std::endl; 
        std::cout << "time elapsed " << std::fixed << std::setprecision(3) << timeElapsed << std::endl;
    }

    float timeElapsed;
};
