#include <Arduino.h>
#include "DeviceConfig.h"
#include "Memory.h"

#ifndef IO_h
#define IO_h

class IO {  
  public:
    IO(DeviceConfig* deviceConfig, Memory* memory);
    
    void configureIO(); // call this in setup, or after deviceConfig changes
    void printIO(); // writes I/O configuration to the serial port
    void scanInputs(); // call before logic scan
    void scanOutputs(); // call after logic scan
    void turnOutputsOff();
  private:
    DeviceConfig* _deviceConfig;
    Memory* _memory;
    
    void printPinConfig(int pin);
};

#endif
