#include "Program.h"
#include <Arduino.h>
#include <EEPROM.h>

Program::Program(int eepromAddress) {
  _eepromAddress = eepromAddress;
  _programSize = 0;
}

void Program::setProgramSize(int value) {
  if(value < 0 || value >= MAX_PROGRAM_SIZE) {
    return;
  }
  _programSize = value;
}

int Program::getProgramSize() {
  return _programSize;
}

void Program::storeByte(byte value, int index) {
  if(index < 0 || index >= _programSize) {
    return;
  }
  _programMemory[index] = value;
}

byte Program::getByte(int index) {
  if(index < 0 || index >= _programSize) {
    return 0;
  }
  return _programMemory[index];
}

void Program::printRuntimeId() {
  Serial.print(F("RuntimeId="));
  int endPlusOne = RUNTIME_ID_INDEX + GUID_LENGTH;
  if(_programSize < endPlusOne) {
    printEmptyGuid();
  }
  else {
    printGuidAsHex(RUNTIME_ID_INDEX);
  }
  Serial.println();
}

void Program::printVersionId() {
  Serial.print(F("RuntimeVersionId="));
  int endPlusOne = VERSION_ID_INDEX + GUID_LENGTH;
  if(_programSize < endPlusOne) {
    printEmptyGuid();
  }
  else {
    printGuidAsHex(VERSION_ID_INDEX);
  }
  Serial.println();
}

void Program::printEmptyGuid() {
  for(byte i = 0; i < GUID_LENGTH; i++) {
    printByteAsHex(0);
  }
}

void Program::printGuidAsHex(int startIndex) {
  // binary Guid is stored weird (due to .NET and the UUID standard, etc.)
  // Bytes 0 to 3 are reversed
  // Bytes 4 and 5 are reversed
  // Bytes 6 and 7 are reversed
  // Bytes 8 through 15 are in the right order
  for(int i = startIndex + 3; i >= startIndex; i--) {
    printByteAsHex(_programMemory[i]);
  }
  printByteAsHex(_programMemory[startIndex + 5]);
  printByteAsHex(_programMemory[startIndex + 4]);
  printByteAsHex(_programMemory[startIndex + 7]);
  printByteAsHex(_programMemory[startIndex + 6]);
  for(int i = startIndex + 8; i < startIndex + 16; i++) {
    printByteAsHex(_programMemory[i]);
  }
}

void Program::printByteAsHex(byte value) {
  if (value < 16) {
    Serial.print("0"); // prints leading 0, which the following line, inexplicably, does not
  }
  Serial.print(value, HEX);
}

boolean Program::readFromEeprom() {
  int address = _eepromAddress;
  byte magicValue = EEPROM.read(address++);
  if(magicValue != MAGIC_VALUE) {
    // probably not initialized
    return false;
  }
  byte lengthHi = EEPROM.read(address++);
  byte lengthLo = EEPROM.read(address++);
  int length = (lengthHi << 8) + lengthLo;
  if(length < 0 || length > MAX_PROGRAM_SIZE) {
    return false;
  }
  _programSize = length;
  for(int i = 0; i < length; i++) {
    _programMemory[i] = EEPROM.read(address++);
  }
  return true;
}

void Program::writeToEeprom() {
  int address = _eepromAddress;
  EEPROM.write(address++, MAGIC_VALUE);
  byte lengthHi = _programSize >> 8;
  byte lengthLo = _programSize & 0x00ff;
  EEPROM.write(address++, lengthHi);
  EEPROM.write(address++, lengthLo);
  for(int i = 0; i < _programSize; i++) {
    EEPROM.write(address++, _programMemory[i]);
  }
}

