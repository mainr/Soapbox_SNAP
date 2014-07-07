#include <Arduino.h>
#include "BoardSelect.h"

#ifndef DeviceConfig_h
#define DeviceConfig_h

const int DEVICE_CONFIG_EEPROM_BYTES = 4;

// 0 and 1 are serial port pins and 13 is a status LED
const int MIN_PIN_NUMBER = 2; // digital I/O
const int MAX_PIN_NUMBER = 12;

class DeviceConfig {  
  public:
    DeviceConfig(int eepromAddress);
    void readFromEeprom();
    void writeToEeprom();
    
    boolean isOutput(int pin);
    void setOutput(int pin, boolean isOutput);
    void reset(); // resets configuration to all inputs
    
  private:
    int _eepromAddress;
    byte _config[DEVICE_CONFIG_EEPROM_BYTES]; // copy of EEPROM data
};

#endif
