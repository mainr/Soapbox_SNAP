#include <Arduino.h>
#include "IO.h"
#include "Memory.h"
#include "Program.h"
#include "Engine.h"

#ifndef SerialPort_h
#define SerialPort_h

const int COMMAND_LINE_BUFFER_SIZE = 128;
const char CARRIAGE_RETURN = 13;
const char LINE_FEED = 10;
const char BACKSPACE = 8;
const char NULL_CHAR = 0;

const char PROTOCOL_VERSION[] = "1.0"; // specifies the serial port protocol version, for compatibilty checking
const char DEVICE_NAME[] = "Arduino";

class SerialPort {  
  public:
    SerialPort(IO* io, Memory* memory, DeviceConfig* deviceConfig, Program* program, Engine* engine);
    void setupSerialPort();
    void readAndWrite();
  private:
    IO* _io;
    Memory* _memory;
    DeviceConfig* _deviceConfig;
    Program* _program;
    Engine* _engine;
    char _buffer[COMMAND_LINE_BUFFER_SIZE+1]; // plus 1 is to hold the terminating null
    int _charsInBuffer;
    
    void clearBuffer();
    void processCharacter(char c);
    void processLine(String &line);
    
    void printEOM();
    void printSuccess(boolean success);
    
    void download(String &line, boolean patch);
    
    void factoryReset();
    void configReset();
};

#endif
