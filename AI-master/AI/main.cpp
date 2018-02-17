#include "AIAgent.h"

#include <iostream>
#include <thread>         // std::thread
#include <chrono>
#include <mutex>

bool g_isGameOver = false;

AIAgent* g_agent = nullptr;
std::mutex g_num_mutex;

void InputListener()
{
    char userInput[255];
    while (!g_isGameOver)
    {
        std::cout << "press d to display debug info" << std::endl;
        std::cout << "press q to exit" << std::endl;
        
        if (!g_agent->IsDead())
        {
            std::cout << "type attack to attack" << std::endl;
            std::cout << "type idle to go to idle" << std::endl;
            std::cout << "type dead to die" << std::endl;
        }

        std::cin >> userInput;
        switch (userInput[0])
        {
        case 'd':
            if (g_agent)
            {
                g_agent->DebugDisplay();
            }  
            else
                std::cout << "all my agents are dead, push me to the edge" << std::endl;
            break;
        case 'q':
            g_isGameOver = true;
            break;
        default:
            break;
        }

        //state related
        if (g_agent && !g_agent->IsDead())
        {
            if (strcmp(userInput, "idle") == 0)
            {
                g_agent->GoToIdle();
            }
            else if (strcmp(userInput, "attack") == 0)
            {
                g_agent->Attack(nullptr);
            }
            else if (strcmp(userInput, "dead") == 0)
            {
                g_agent->Die();
            }
        }
        
        std::this_thread::sleep_for(std::chrono::milliseconds(10));
    }
}

int main()
{
    g_agent = new AIAgent();
    std::thread inputListener(InputListener);

    while (!g_isGameOver)
    {
        g_num_mutex.lock();
        g_agent->Update(0.0167f);
        g_num_mutex.unlock();
        std::this_thread::sleep_for(std::chrono::milliseconds(10));
    }

    inputListener.join();
    
    return 0;
}