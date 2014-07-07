#include <EEPROM.h>

#include "Program.h"
#include "DeviceConfig.h"
#include "Memory.h"
#include "IO.h"
#include "Engine.h"
#include "SerialPort.h"
#include "StatusLED.h"
#include "TimerOne.h"

DeviceConfig deviceConfig = DeviceConfig(0); // uses first bytes of EEPROM
Program program = Program(DEVICE_CONFIG_EEPROM_BYTES + ENGINE_EEPROM_BYTES);
Memory memory = Memory(); // these are the current values of coils and analog values, etc.
IO io = IO(&deviceConfig, &memory);
Engine engine = Engine(DEVICE_CONFIG_EEPROM_BYTES, &program, &memory);
SerialPort serialPort = SerialPort(&io, &memory, &deviceConfig, &program, &engine);
StatusLED statusLED = StatusLED(13, &engine);

void setup() {        
  deviceConfig.readFromEeprom(); 
  if(program.readFromEeprom()) {
    engine.readFromEeprom(); // reads engine running status
  }
  engine.preScan();
  io.configureIO();
  statusLED.init();
  serialPort.setupSerialPort();  
  Timer1.initialize(100000); // every 0.1 seconds
  Timer1.attachInterrupt(timerISR);
}

void timerISR() {
  statusLED.timerISR();
}

void loop() {  
  io.scanInputs();
  engine.solveLogic();
  if(engine.getStatus() && engine.getErrorCode() == ERROR_NONE) {
    io.scanOutputs();
  }
  else {
    io.turnOutputsOff();
  }
  serialPort.readAndWrite();
  //delay(10);
}
