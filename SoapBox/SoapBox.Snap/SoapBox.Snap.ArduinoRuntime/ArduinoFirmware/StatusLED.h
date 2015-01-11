#include <Arduino.h>
#include "Engine.h"

#ifndef StatusLED_h
#define StatusLED_h

const int STATUS_LED_TIMER_HZ = 490; // fixed at this due to Timer1 being used for PWM on pins 9 and 10
const int STATUS_LED_10_PERCENT = STATUS_LED_TIMER_HZ / 10;

class StatusLED {
  public:
    StatusLED(int ledPin, Engine* engine);
    void init(); // call from setup
    void timerISR();
    
  private:
    int _ledPin;
    Engine* _engine;
    byte _innerCounter; // counts 0 to 49
    byte _counter; // counts 0 to 9
    
    void countTenth();
};
#endif
