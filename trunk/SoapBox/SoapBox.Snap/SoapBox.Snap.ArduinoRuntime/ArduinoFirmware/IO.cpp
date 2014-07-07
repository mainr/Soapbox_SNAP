#include "IO.h"

IO::IO(DeviceConfig* deviceConfig, Memory* memory) {
  _deviceConfig = deviceConfig;
  _memory = memory;
}

void IO::configureIO() {
  for(int pin = MIN_PIN_NUMBER; pin <= MAX_PIN_NUMBER; pin++) {
    if(_deviceConfig->isOutput(pin)) {
      pinMode(pin, OUTPUT);
    }
    else {
      pinMode(pin, INPUT);
    }
  }
}

void IO::printIO() {
  for(int pin = MIN_PIN_NUMBER; pin <= MAX_PIN_NUMBER; pin++) {
    printPinConfig(pin);
  }
  for(int analog = MIN_ANALOG_INPUT; analog <= MAX_ANALOG_INPUT; analog++) {
    int address = analog - MIN_ANALOG_INPUT;
    Serial.print("A");
    Serial.print(analog);
    Serial.print(", ");
    Serial.print(address);
    Serial.println(", analogInput");
  }
}

void IO::printPinConfig(int pin) {
  // format for each line is: name, address, type
  // Type is: input, output, analogInput, analogOutput
  
  int address = getAddress(pin);
  
  Serial.print("pin");
  Serial.print(pin);
  Serial.print(", ");
  Serial.print(address);
  Serial.print(", ");
  if(_deviceConfig->isOutput(pin)) {
    Serial.print("output");
  }
  else {
    Serial.print("input");
  }
  Serial.println();
}

int IO::getAddress(int pin) {
  return pin - MIN_PIN_NUMBER;
}

void IO::scanInputs() {
  for(int pin = MIN_PIN_NUMBER; pin <= MAX_PIN_NUMBER; pin++) {
    if(!_deviceConfig->isOutput(pin)) {
      int address = getAddress(pin);
      if(digitalRead(pin) == HIGH) {
        _memory->writeBoolean(address, true);
      }
      else {
        _memory->writeBoolean(address, false);
      }
    }
  }
  for(int analogPin = MIN_ANALOG_INPUT; analogPin <= MAX_ANALOG_INPUT; analogPin++) {
    byte address = analogPin - MIN_ANALOG_INPUT;
    int value = analogRead(analogPin);
    NumericMemoryValue memoryValue;
    memoryValue.isFloat = false;
    memoryValue.value.longValue = value;
    _memory->writeNumeric(address, memoryValue);
  }
}

void IO::scanOutputs() {
  for(int pin = MIN_PIN_NUMBER; pin <= MAX_PIN_NUMBER; pin++) {
    if(_deviceConfig->isOutput(pin)) {
      int address = getAddress(pin);
      boolean value = _memory->readBoolean(address);
      if(value) {
        digitalWrite(pin, HIGH);
      }
      else {
        digitalWrite(pin, LOW);
      }
    }
  }
}

void IO::turnOutputsOff() {
  for(int pin = MIN_PIN_NUMBER; pin <= MAX_PIN_NUMBER; pin++) {
    if(_deviceConfig->isOutput(pin)) {
      int address = getAddress(pin);
      digitalWrite(pin, LOW);
    }
  }
}
