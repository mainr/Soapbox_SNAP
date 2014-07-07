#include "DeviceConfig.h"
#include <Arduino.h>
#include <EEPROM.h>

// EEPROM Map (reserved 4 bytes)
// Byte 0:
//   bit 2 to 7 = pinMode for pins 2 through 7 (pins 0, 1 are serial)
// Byte 1:
//   bit 0 to 4 = pinMode for pins 8 to 12 (pin 13 is status LED)
// Byte 2:
//   reserved
// Byte 3:
//   reserved

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

boolean DeviceConfig::isOutput(int pin) {
  if(pin < MIN_PIN_NUMBER || pin > MAX_PIN_NUMBER) return false;
  int addressByteNumber = pin >> 3;
  int addressBitNumber = pin & 7;
  return bitRead(_config[addressByteNumber], addressBitNumber);
}

void DeviceConfig::setOutput(int pin, boolean isOutput) {
  if(pin < MIN_PIN_NUMBER || pin > MAX_PIN_NUMBER) return;
  int addressByteNumber = pin >> 3;
  int addressBitNumber = pin & 7;
  if(isOutput) {
    bitSet(_config[addressByteNumber], addressBitNumber);
  }
  else {
    bitClear(_config[addressByteNumber], addressBitNumber);
  }
}

void DeviceConfig::reset() {
  for(int i = MIN_PIN_NUMBER; i <= MAX_PIN_NUMBER; i++) {
    setOutput(i, false);
  }
}
