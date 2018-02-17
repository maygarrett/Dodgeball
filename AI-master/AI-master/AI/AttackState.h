#pragma once

#include "BaseState.h"
#include <iostream>
#include <iomanip>
class AttackState : public BaseState
{
public:
    AttackState(AIAgent* owner);

    void Init() override;
    void Shutdown() override;

    void OnEnter() override;
    void OnExit() override;
    void Update(float deltaTime) override;

    FSM_State GetStateName() const override { return FSM_Attack; }

    void DebugDisplay() const override
    {
        std::cout << "Attack State" << std::endl;
        std::cout << "time elapsed " << std::fixed << std::setprecision(3) << timeElapsed << std::endl;
    }

    float timeElapsed;
};

