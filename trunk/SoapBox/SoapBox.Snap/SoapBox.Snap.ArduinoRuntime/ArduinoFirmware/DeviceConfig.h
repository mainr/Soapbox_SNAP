#include <Arduino.h>
#include "BoardSelect.h"

#ifndef DeviceConfig_h
#define DeviceConfig_h

enum pin_config {
  pin_config_input = 0xFF,
  pin_config_output = 1,
  pin_config_pwm = 2,
};

class DeviceConfig {  
  public:
    DeviceConfig(int eepromAddress);
    void readFromEeprom();
    void writeToEeprom();
    
    boolean isInput(int pin);
    void setInput(int pin);
    boolean isOutput(int pin);
    void setOutput(int pin);
    boolean validPwmPin(int pin);
    boolean isPwm(int pin);
    void setPwm(int pin);
    void reset(); // resets configuration to all inputs
    
    int getAddress(int pin); // returns boolean address in memory for digital I/O pin
    byte getPwmAddress(int pin); // returns numeric address in memory for analog PWM pin
    
  private:
    int _eepromAddress;
    byte _config[DEVICE_CONFIG_EEPROM_BYTES]; // copy of EEPROM data
};

#endif
