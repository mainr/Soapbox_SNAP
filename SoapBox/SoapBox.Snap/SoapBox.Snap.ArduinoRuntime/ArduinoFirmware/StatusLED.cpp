#include "StatusLED.h"

StatusLED::StatusLED(int ledPin, Engine* engine) {
  _ledPin = ledPin;
  _engine = engine;
  _counter = 0; // counts 0 to 9
}

void StatusLED::init() {
  pinMode(_ledPin, OUTPUT); // this is used as a runtime status LED
}

void StatusLED::timerISR() {
  _counter++;
  if(_counter >= 10) {
    _counter = 0;
  }
  boolean state = false;
  byte errorCode = _engine->getErrorCode();
  if(errorCode == ERROR_NONE) {
    if(_engine->getStatus())
    {
      state = _counter != 9; // running
    }
    else {
      state = _counter == 0; // not running
    }
  }
  else {
    state = digitalRead(_ledPin) ^ 1;
  }
  digitalWrite(_ledPin, state);
}
