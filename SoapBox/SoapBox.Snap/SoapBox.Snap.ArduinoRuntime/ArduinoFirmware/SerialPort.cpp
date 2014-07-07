#include "SerialPort.h"

SerialPort::SerialPort(IO* io, Memory* memory, DeviceConfig* deviceConfig, Program* program, Engine* engine) {
  _io = io;
  _memory = memory;
  _deviceConfig = deviceConfig;
  _program = program;
  _engine = engine;
  clearBuffer();
}

void SerialPort::setupSerialPort() {
  Serial.begin(115200);
}

void SerialPort::readAndWrite() {
  byte toRead = Serial.available();
  if(toRead > 0) {
    char inputBuffer[64];
    byte bytesRead = Serial.readBytes(inputBuffer, toRead);
    
    if(bytesRead + _charsInBuffer > COMMAND_LINE_BUFFER_SIZE) {
      // some kind of error, not much we can do, so reset
      clearBuffer();
      Serial.println();
    }
    else {
      for(int i = 0; i < bytesRead; i++) {
        char c = inputBuffer[i];
        processCharacter(c);
      }
    }
  }
}

void SerialPort::clearBuffer() {
  _charsInBuffer = 0;
  _buffer[0] = NULL_CHAR;
}

void SerialPort::processCharacter(char c) {
  if(c < 0) {
    // ignore (seems to fix spurious characters that get sent when .NET starting up)
  }
  else if(c == BACKSPACE) {
    if(_charsInBuffer > 0) {
      _charsInBuffer--;
      _buffer[_charsInBuffer] = NULL_CHAR;
    }
  }
  else if(c == LINE_FEED) {
    if(_charsInBuffer > 0 && _buffer[_charsInBuffer] == CARRIAGE_RETURN) {
      _charsInBuffer--;
      _buffer[_charsInBuffer] = NULL_CHAR;
    }
    String line = String(_buffer);
    clearBuffer();
    line.trim();
    line.toLowerCase();
    processLine(line);
  }
  else {
    _buffer[_charsInBuffer] = c;
    _charsInBuffer++;
    _buffer[_charsInBuffer] = NULL_CHAR;
  }
}

void SerialPort::processLine(String &line) {
  if(line.equals("information")) {
    Serial.println(F("SoapBox Snap Arduino Runtime"));
    Serial.print(F("Protocol Version="));
    Serial.println(PROTOCOL_VERSION);
    Serial.print(F("Booleans="));
    Serial.println(NUM_BOOLEANS);
    Serial.print(F("Numerics="));
    Serial.println(NUM_NUMERICS);
    Serial.print(F("Strings="));
    Serial.println(NUM_STRINGS);
    Serial.print(F("Max Program Size="));
    Serial.println(MAX_PROGRAM_SIZE);
    Serial.print(F("Current Program Size="));
    Serial.println(_program->getProgramSize());
  }
  else if(line.equals("device-config")) {
    Serial.print(F("Device Name="));
    Serial.println(DEVICE_NAME);
    // syntax here is name, address, pinMode
    _io->printIO();
  }
  else if(line.equals("factory-reset")) {
    factoryReset();
    printSuccess(true); 
  }
  else if(line.equals("config-reset")) {
    configReset();
    printSuccess(true); 
  }
  else if(line.startsWith("config-output ")) {
    String p = line.substring(14);
    int pin = p.toInt();
    _deviceConfig->setOutput(pin, true);
    _deviceConfig->writeToEeprom();
    _io->configureIO();
    printSuccess(true); 
  }
  else if(line.equals("status")) {
    Serial.print(F("Running=")); // status of runtime (running or stopped)
    if(_engine->getStatus()) {
      Serial.println(F("true"));
    }
    else {
      Serial.println(F("false"));
    }
  }
  else if(line.equals("start")) {
    printSuccess(_engine->startEngine()); // start runtime
  }
  else if(line.equals("stop")) {
    printSuccess(_engine->stopEngine()); // stop runtime
  }
  else if(line.startsWith("download ")) {
    download(line, false);
  }
  else if(line.startsWith("patch ")) {
    download(line, true);
  }
  else if(line.equals("runtime-id")) {
    _program->printRuntimeId();
  }
  else if(line.equals("version-id")) {
    _program->printVersionId();
  }
  else if(line.startsWith("read b")) {
    String addr = line.substring(6);
    int address = addr.toInt();
    if(_memory->readBoolean(address)) {
      Serial.println(F("1"));
    }
    else {
      Serial.println(F("0"));
    }
  }
  else if(line.startsWith("write b")) {
    String addrAndValue = line.substring(7);
    addrAndValue.trim();
    int separatorLocation = addrAndValue.indexOf(' ');
    if(separatorLocation > 0) {
      String addr = addrAndValue.substring(0,separatorLocation);
      String val = addrAndValue.substring(separatorLocation+1);
      int address = addr.toInt();
      boolean value = false;
      if(val.startsWith("1")) {
        value = true;
      }
      _memory->writeBoolean(address, value);
    }
  }
  else if(line.startsWith("read n")) {
    String addr = line.substring(6);
    int address = addr.toInt();
    NumericMemoryValue readResult = _memory->readNumeric((byte)address);
    if(readResult.isFloat) {
      Serial.println(readResult.value.floatValue, 6);
    }
    else {
      Serial.println(readResult.value.longValue);
    }
  }
  else if(line.startsWith("write n")) {
    String addrAndValue = line.substring(7);
    addrAndValue.trim();
    int separatorLocation = addrAndValue.indexOf(' ');
    if(separatorLocation > 0) {
      String addr = addrAndValue.substring(0,separatorLocation);
      String val = addrAndValue.substring(separatorLocation+1);
      if(val.length() < 30) {
        char buffer[32];
        val.toCharArray(buffer, sizeof(buffer));
        NumericMemoryValue writeValue;
        if(val.indexOf('.') == -1) {
          // assume it's an integer
          writeValue.isFloat = false;
          writeValue.value.longValue = atol(buffer);
        }
        else {
          // assume it's a float
          writeValue.isFloat = true;
          writeValue.value.floatValue = atof(buffer);
        }
        int address = addr.toInt();
        _memory->writeNumeric((byte)address, writeValue);
      }
    }
  }
  else {
    Serial.print(F("Error=no such command: "));
    Serial.println(line);
  }
  printEOM();
}

void SerialPort::printEOM() {
  Serial.println(F("EOM"));
}

void SerialPort::printSuccess(boolean success) {
  Serial.print(F("Success="));
  if(success) {
    Serial.println(F("true"));
  }
  else {
    Serial.println(F("false"));
  }
}

void SerialPort::download(String &line, boolean patch) {
    int lenIndex = patch ? 6 : 9; // download is 8 chars, patch is 5
    String len = line.substring(lenIndex);
    len.trim();
    int length = len.toInt(); // length is in bytes
    _program->setProgramSize(length);
    printSuccess(true); // means start transmitting program
    printEOM();
    if(!patch) {
      _engine->stopEngine();
    }
    int totalBytesRead = 0;
    char buffer[1];
    while(totalBytesRead < length) {
      if(Serial.readBytes(buffer, 1) == 1) {
        _program->storeByte((byte)(buffer[0]), totalBytesRead); // just puts it in memory
        totalBytesRead++;
      }
    }
    _program->writeToEeprom(); // stores it in persistent storage
    printSuccess(true);
    if(!patch) {
      _engine->startEngine();
    }
}

void SerialPort::factoryReset() {
  _engine->stopEngine();
  configReset();
  _program->setProgramSize(0);
  _program->writeToEeprom(); // stores it in persistent storage
}

void SerialPort::configReset() {
  _deviceConfig->reset();
  _deviceConfig->writeToEeprom();
  _io->configureIO();
}
