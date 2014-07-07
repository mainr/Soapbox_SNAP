#include <Arduino.h>
#include "Engine.h"

#ifndef StatusLED_h
#define StatusLED_h

class StatusLED {
  public:
    StatusLED(int ledPin, Engine* engine);
    void init(); // call from setup
    void timerISR();
    
  private:
    int _ledPin;
    Engine* _engine;
    byte _counter;
    
};
#endif
