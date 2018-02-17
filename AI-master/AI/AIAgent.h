#pragma once

class StateManager;
class AIAgent
{
public:
    AIAgent();
    ~AIAgent();

    void Update(float deltaTime);

    int GetHealth() const { return m_health; }
    unsigned int GetAmmo() const { return m_ammo; }

    int GetAttackValue() const { return m_skills.m_attack; }
    int GetDefendValue() const { return m_skills.m_defend; }

    void Reload();

    void GoToIdle();
    void Attack(AIAgent* target);
    void Die();

    bool IsDead() const;
    
    void Defend();
    void TakeDamage(AIAgent* instigator);

    void DebugDisplay() const;
    
protected:
    int m_health;
    unsigned int m_ammo;

    const int MAX_ATTACK = 100;
    const int MAX_DEFEND = 100;

    struct CombatSkills
    {
        int m_attack;
        int m_defend;
    };

    CombatSkills m_skills;
    StateManager* m_stateManager;
};
