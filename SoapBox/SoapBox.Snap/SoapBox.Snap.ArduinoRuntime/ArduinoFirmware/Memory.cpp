#include "Memory.h"

Memory::Memory() {
  memset(_booleans,0,sizeof(_booleans));
  memset(_isNumberAFloat,0,sizeof(_isNumberAFloat));
  memset(_numerics,0,sizeof(_numerics));
}

boolean Memory::readBoolean(int address) {
  if(address < 0 || address >= NUM_BOOLEANS) return false;
  byte byteValue = _booleans[address >> 3];
  byte bitValue = (address & 7);
  byte mask = 1 << bitValue;
  if(byteValue & mask) return true;
  return false;
}

boolean Memory::writeBoolean(int address, boolean value) {
  //Serial.print("address: "); Serial.print(address); Serial.print(", value: "); Serial.println(value);
  if(address < 0 || address >= NUM_BOOLEANS) return false;
  int byteAddress = (address >> 3);
  byte bitValue = (address & 7);
  byte mask = (1 << bitValue);
  if(value) {
    _booleans[byteAddress] = _booleans[byteAddress] | mask;
  }
  else {
    _booleans[byteAddress] = _booleans[byteAddress] & ~mask;
  }
  return true;
}

NumericMemoryValue Memory::readNumeric(byte address) {
  NumericMemoryValue result;
  if(address >= NUM_NUMERICS) {
    result.isFloat = false;
    result.value.longValue = 0L;
    return result;
  }
  byte byteValue = _isNumberAFloat[address >> 3];
  byte bitValue = address & 7;
  byte mask = 1 << bitValue;
  if(byteValue & mask) {
    result.isFloat = true;
  }
  else {
    result.isFloat = false;
  }
  result.value = _numerics[address];
  return result;
}

boolean Memory::writeNumeric(byte address, NumericMemoryValue value) {
  if(address >= NUM_NUMERICS) return false;
  byte byteAddress = address >> 3;
  byte bitValue = address & 7;
  byte mask = 1 << bitValue;
  if(value.isFloat) {
    _isNumberAFloat[byteAddress] = _isNumberAFloat[byteAddress] | mask;
  }
  else {
    _isNumberAFloat[byteAddress] = _isNumberAFloat[byteAddress] & ~mask;
  }
  _numerics[address] = value.value;
  return true;
}
