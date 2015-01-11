#include "DeviceConfig.h"
#include <Arduino.h>
#include <EEPROM.h>

DeviceConfig::DeviceConfig(int eepromAddress) {
  _eepromAddress = eepromAddress;
}

void DeviceConfig::readFromEeprom() {
  for (int i = 0; i < DEVICE_CONFIG_EEPROM_BYTES; i++)
    _config[i] = EEPROM.read(_eepromAddress + i);
}

void DeviceConfig::writeToEeprom() {
  for (int i = 0; i < DEVICE_CONFIG_EEPROM_BYTES; i++) {
    EEPROM.write(_eepromAddress + i, _config[i]);
  }
}

boolean DeviceConfig::isInput(int pin) {
  if(pin < MIN_PIN_NUMBER || pin > MAX_PIN_NUMBER || pin == STATUS_LED_PIN) return false;
  return _config[pin] == pin_config_input;
}

void DeviceConfig::setInput(int pin) {
  if(pin < MIN_PIN_NUMBER || pin > MAX_PIN_NUMBER || pin == STATUS_LED_PIN) return;
  _config[pin] = pin_config_input;
}

boolean DeviceConfig::isOutput(int pin) {
  if(pin < MIN_PIN_NUMBER || pin > MAX_PIN_NUMBER || pin == STATUS_LED_PIN) return false;
  return _config[pin] == pin_config_output;
}

void DeviceConfig::setOutput(int pin) {
  if(pin < MIN_PIN_NUMBER || pin > MAX_PIN_NUMBER || pin == STATUS_LED_PIN) return;
  _config[pin] = pin_config_output;
}

boolean DeviceConfig::isPwm(int pin) {
  if(!validPwmPin(pin)) return false;
  return _config[pin] == pin_config_pwm;
}

void DeviceConfig::setPwm(int pin) {
  if(!validPwmPin(pin)) return;
  _config[pin] = pin_config_pwm;
}

void DeviceConfig::reset() {
  for(int i = MIN_PIN_NUMBER; i <= MAX_PIN_NUMBER; i++) {
    if(i != STATUS_LED_PIN) {
      setInput(i);
    }
  }
}

int DeviceConfig::getAddress(int pin) {
  // return a boolean address
  return pin - MIN_PIN_NUMBER;
}

byte DeviceConfig::getPwmAddress(int pin) {
  if(!isPwm(pin)) {
    return 0; // this is actually an error... not sure what to do about it here
  }
  // return a numeric address
  // first N numeric addresses are used by the analog inputs
  byte pwmAddress = MAX_ANALOG_INPUT + 1;
  // assign addresses in order of pwm pins in use
  for(int i = 0; i < PWM_PIN_COUNT; i++) {
    int pwmPin = PWM_PINS[i];
    if(pwmPin == pin) {
      return pwmAddress;
    }
    if(isPwm(pwmPin)) {
      pwmAddress++;
    }
  }
  return 0;
}

boolean DeviceConfig::validPwmPin(int pin) {
  for(int i = 0; i < PWM_PIN_COUNT; i++) {
    if(PWM_PINS[i] == pin) {
      return true;
    }
  }
  return false;
}
